using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using NBlog.Server.Application;
using nBlogCmd.Application;
using Toolbox.Extensions;

namespace NBlog.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Option option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            IHost host = CreateHostBuilder(args, option).Build();

            ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
            option.DumpConfigurations(logger);

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, Option option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(option);

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

                    if (!option.InstrumentationKey.IsEmpty())
                    {
                        config.AddApplicationInsights(option.InstrumentationKey);
                        config.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}