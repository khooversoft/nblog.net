using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlogCmd.Application;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Tools;

namespace nBlogCmd.Activities
{
    internal class BuildArticlesActivity
    {
        private readonly ILogger<BuildArticlesActivity> _logger;
        private readonly Option _option;

        public BuildArticlesActivity(Option option, ILogger<BuildArticlesActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public async Task Build(CancellationToken token)
        {
            ActionBlock<string> activities = new ActionBlock<string>(x => BuildPackage(x, token), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            string[] specFilePaths = Directory.GetFiles(_option.SourceFolder!, "*.json", SearchOption.AllDirectories);

            foreach (var filePath in specFilePaths)
            {
                await activities.SendAsync(filePath);
            }

            activities.Complete();
            await activities.Completion;
        }

        private void BuildPackage(string specFilePath, CancellationToken token)
        {
            _logger.LogInformation($"Building {specFilePath}");

            int total = 0;
            int count = 0;

            using Timer timer = new Timer(_ => log(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            new ArticlePackageBuilder()
                .SetSpecFile(specFilePath)
                .SetBuildFolder(ArticleConstants.Folders.GetPackageFolder(_option.BuildFolder!))
                .SetObjFolder(ArticleConstants.Folders.GetObjFolder(_option.BuildFolder!))
                .Build(x =>
                {
                    total = x.Total;
                    Interlocked.Add(ref count, x.Count);
                }, token);

            log("Final");

            void log(string? prefix = null)
            {
                _logger.LogInformation($"Build: {(prefix != null ? prefix + " " : string.Empty)}{specFilePath}, Count={count}, Total={total}");
            }
        }
    }
}