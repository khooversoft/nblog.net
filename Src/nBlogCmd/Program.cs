﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nBlog.sdk.Client;
using nBlog.sdk.Store;
using nBlogCmd.Activities;
using nBlogCmd.Application;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Extensions;

[assembly: InternalsVisibleTo("nBlogCmd.Test")]

namespace nBlogCmd
{
    class Program
    {
        private const int _ok = 0;
        private const int _error = 1;
        private readonly string _programTitle = $"nBlog CLI Build & Release - Version {Assembly.GetExecutingAssembly().GetName().Version}";

        static async Task<int> Main(string[] args)
        {
            try
            {
                return await new Program().Run(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("*Error: " + ex.Message + ex.ToString());
                DisplayStartDetails(args);
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine("Unhanded exception: " + ex.ToString());
            }

            return _error;
        }

        private static void DisplayStartDetails(string[] args) => Console.WriteLine($"Arguments: {string.Join(", ", args)}");

        private async Task<int> Run(string[] args)
        {
            Console.WriteLine(_programTitle);
            Console.WriteLine();

            Option option = new OptionBuilder()
                .SetArgs(args)
                .Build();

            if (option.Help)
            {
                option.GetHelp()
                    .Append(string.Empty)
                    .ForEach(x => Console.WriteLine(x));

                return _ok;
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            using (ServiceProvider container = BuildContainer(option))
            {
                ILogger<Program> logger = container
                    .GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

                option.DumpConfigurations(logger);

                Console.CancelKeyPress += (object? _, ConsoleCancelEventArgs e) =>
                {
                    e.Cancel = true;
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Canceling...");
                };

                var executeQueue = new ActionBlock<Func<Task>>(async x => await x());

                if (option.Build) await executeQueue.SendAsync(() => container.GetRequiredService<ClearBuildFolderActivity>().Clear());

                if (option.Build) await executeQueue.SendAsync(() => container.GetRequiredService<BuildArticlesActivity>().Build(cancellationTokenSource.Token));

                if (option.Build) await executeQueue.SendAsync(() => container.GetRequiredService<BuildArticleIndexActivity>().Build(cancellationTokenSource.Token));
                if (option.Build) await executeQueue.SendAsync(() => container.GetRequiredService<BuildWordIndexActivity>().Build(cancellationTokenSource.Token));

                if (option.Upload) await executeQueue.SendAsync(() => container.GetRequiredService<UploadArticlesActivity>().Upload(cancellationTokenSource.Token));
                if (option.Upload) await executeQueue.SendAsync(() => container.GetRequiredService<UploadDirectoryActivity>().Upload(cancellationTokenSource.Token));

                executeQueue.Complete();
                await executeQueue.Completion;
            }

            Console.WriteLine();
            Console.WriteLine("Completed");
            return _ok;
        }

        private ServiceProvider BuildContainer(Option option)
        {
            var service = new ServiceCollection();

            service.AddLogging(x =>
            {
                x.AddConsole();
                x.AddDebug();
            });

            service.AddSingleton(option);

            service.AddSingleton<ClearBuildFolderActivity>();
            service.AddSingleton<BuildArticlesActivity>();

            service.AddSingleton<BuildArticleIndexActivity>();
            service.AddSingleton<BuildWordIndexActivity>();

            service.AddSingleton<UploadArticlesActivity>();
            service.AddSingleton<UploadDirectoryActivity>();

            service.AddHttpClient<IArticleClient, ArticleClient>(setupHttpClient);
            service.AddHttpClient<IDirectoryClient, DirectoryClient>(setupHttpClient);

            return service.BuildServiceProvider();

            void setupHttpClient(HttpClient httpClient)
            {
                httpClient.BaseAddress = new Uri(option.BlogStoreOption.StoreUrl);
                httpClient.DefaultRequestHeaders.Add(StoreConstants.ApiKeyName, option.BlogStoreOption.ApiKey);
            }
        }
    }
}
