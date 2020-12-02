using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlogCmd.Application;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Tools;

namespace nBlogCmd.Activities
{
    internal class BuildActivity
    {
        private readonly ILogger<BuildActivity> _logger;
        private readonly Option _option;

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

        private void BuildPackage(string specFilePath, CancellationToken token)
        {
            _logger.LogInformation($"Building {specFilePath}");

            new ArticlePackageBuilder()
                .SetSpecFile(specFilePath)
                .SetBuildFolder(_option.BuildFolder!)
                .Build(x => _logger.LogInformation($"Build: {specFilePath}, Count={x.Count}, Total={x.Total}"), token);
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
    }
}