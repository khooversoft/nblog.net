using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBlog.Server.Application;
using nBlogCmd.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NBlog.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Option option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            CreateHostBuilder(args, option)
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, Option option) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(option);
                })
                .ConfigureLogging(config =>
                {
                    config.AddConsole();
                    config.AddDebug();
                    config.AddFilter(x => true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
