using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nBlog.Store.Application;
using nBlog.Store.Services;
using System;
using System.Collections;
using System.Threading.Tasks;
using Toolbox.Extensions;
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

                    if (!option.InstrumentationKey.IsEmpty())
                    {
                        services.AddApplicationInsightsTelemetry(option.InstrumentationKey);
                    }
                })
                .ConfigureLogging(config =>
                {
                    config.AddConsole();
                    config.AddDebug();
                    config.AddFilter(x => true);

                    config.AddProvider(new TargetBlockLoggerProvider(telemetryBuffer.TargetBlock));

                    if (!option.InstrumentationKey.IsEmpty())
                    {
                        config.AddApplicationInsights(option.InstrumentationKey);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    if (!option.ApplicationUrl.IsEmpty())
                    {
                        webBuilder.UseUrls(option.ApplicationUrl.Split(';', System.StringSplitOptions.RemoveEmptyEntries));
                    }
                });
        }
    }
}