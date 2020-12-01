using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nBlog.sdk.Client;
using nBlog.Store.Application;
using nBlog.Store.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Model;

namespace nBlog.Store.Test.Application
{
    internal class TestWebsiteHost
    {
        protected IHost? _host;
        protected HttpClient? _client;
        private readonly ILogger<TestWebsiteHost> _logger;

        public TestWebsiteHost(ILogger<TestWebsiteHost> logger) => _logger = logger;

        public HttpClient Client => _client ?? throw new ArgumentNullException(nameof(Client));

        public T Resolve<T>() where T : class => _host?.Services.GetService<T>() ?? throw new InvalidOperationException($"Cannot find service {typeof(T).Name}");

        public BlogClient BlogClient => new BlogClient(Client, Resolve<ILoggerFactory>().CreateLogger<BlogClient>());

        public TestWebsiteHost StartApiServer()
        {
            Option option = GetOption();

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
                    services.AddSingleton(option);
                    services.AddSingleton(option.Store);
                });

            _host = host.Start();
            _client = _host.GetTestServer().CreateClient();
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

        private Option GetOption()
        {
            string[] args = new string[]
            {
                "Environment=local",
                "SecretId=nBlogCmd",
                "Store:ContainerName=test"
            };

            return new OptionBuilder()
                .SetArgs(args)
                .Build();
        }
    }
}
