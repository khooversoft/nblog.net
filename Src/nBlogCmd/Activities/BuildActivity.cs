using Microsoft.Extensions.Logging;
using nBlogCmd.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Tools;
using nBlog.sdk.ArticlePackage;
using System.Threading;
using Toolbox.Services;
using Toolbox.Extensions;

namespace nBlogCmd.Activities
{
    internal class BuildActivity
    {
        private readonly Option _option;
        private readonly ILogger<BuildActivity> _logger;

        public BuildActivity(Option option, ILogger<BuildActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public async Task Build(CancellationToken token)
        {
            ClearBuildFolder();
            await RunBuild(token);
        }

        private void ClearBuildFolder()
        {
            _option.BuildFolder.VerifyNotEmpty($"{nameof(_option.BuildFolder)} not specified");

            if (Directory.Exists(_option.BuildFolder)) Directory.Delete(_option.BuildFolder, true);
            Directory.CreateDirectory(_option.BuildFolder);
        }

        private async Task RunBuild(CancellationToken token)
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
            new ArticlePackageBuilder()
                .SetSpecFile(specFilePath)
                .SetBuildFolder(_option.BuildFolder!)
                .Build(x => _logger.LogInformation($"Build: {specFilePath}, Count={x.Count}, Total={x.Total}"), token);
        }
    }
}
