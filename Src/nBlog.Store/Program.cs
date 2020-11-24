using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nBlog.Store.Application;
using nBlog.Store.Services;
using System.Threading.Tasks;
using Toolbox.Logging;

namespace nBlog.Store
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Option option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            IHost host = CreateHostBuilder(args, option)
                .Build();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, Option option)
        {
            TelemetryBuffer telemetryBuffer = new();

            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(option);
                    services.AddSingleton(option.Store);
                    services.AddSingleton(telemetryBuffer);
                })
                .ConfigureLogging(config =>
                {
                    config.AddConsole();
                    config.AddDebug();

                    config.AddProvider(new TargetBlockLoggerProvider(telemetryBuffer.TargetBlock));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}