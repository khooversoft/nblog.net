using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.Store.Test.Application
{
    internal class TestWebsiteHost
    {
        protected IHost? _host;
        protected HttpClient? _client;
        private readonly ILogger<TestWebsiteHost> _logger;

        public TestWebsiteHost(ILogger<TestWebsiteHost> logger)
        {
            _logger = logger;
        }

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public PathFinderClient PathFinderClient => new PathFinderClient(Client, Resolve<ILoggerFactory>().CreateLogger<PathFinderClient>());

        public TestWebsiteHost StartApiServer(RunEnvironment runEnvironment, string? databaseName = null)
        {
            _logger.LogInformation($"{nameof(StartApiServer)}: runEnvironment={runEnvironment}, databaseName={databaseName}");

            IOption option = GetOption(runEnvironment, databaseName);

            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .UseStartup<Startup>();
                })
                .ConfigureLogging(builder => builder.AddDebug())
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(option)
                        .AddSingleton<ICosmosPathFinderOption>(option.Store);
                });

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();

            InitializeDatabase(_host, option).Wait();
            return this;
        }

        public void Shutdown()
        {
            _logger.LogInformation($"{nameof(Shutdown)} - Shutting down");

            Interlocked.Exchange(ref _client, null)?.Dispose();

            var host = Interlocked.Exchange(ref _host, null);
            if (host != null)
            {
                try
                {
                    host.StopAsync(TimeSpan.FromMinutes(10)).Wait();
                }
                catch { }
                finally
                {
                    host.Dispose();
                }
            }
        }

        private IOption GetOption(RunEnvironment runEnvironment, string? databaseName)
        {
            string tempFile = $"PathFinder.Server.Test.{runEnvironment}.{(databaseName ?? "default")}";
            string packageFile = FileTools.WriteResourceToTempFile(tempFile, nameof(TestWebsiteHost), typeof(TestWebsiteHost), GetResourceId());

            string[] args = new string?[]
            {
                $"ConfigFile={packageFile}",
                !databaseName.IsEmpty() ? $"Store:DatabaseName={databaseName}" : null,
            }
            .Where(x => x != null)
            .ToArray()!;

            return new OptionBuilder()
                .SetArgs(args)
                .Build();

            string GetResourceId() => runEnvironment switch
            {
                RunEnvironment.Local => "PathFinder.Server.Test.Application.Local-Config.json",
                RunEnvironment.Dev => "PathFinder.Server.Test.Application.Dev-Config.json",

                _ => throw new InvalidOperationException($"{runEnvironment} is not supported"),
            };
        }

        private static async Task InitializeDatabase(IHost host, IOption option)
        {
            if (!option.InitializeDatabase) return;

            CancellationToken token = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;
            using var scope = host.Services.CreateScope();

            await scope.ServiceProvider.GetRequiredService<IPathFinderStore>()
                .InitializeContainers(token);
        }
    }
}
