using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using nBlogCmd.Application;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace nBlogCmd.Activities
{
    internal class BuildWordIndexActivity
    {
        private readonly ILogger<BuildWordIndexActivity> _logger;
        private readonly Option _option;

        public BuildWordIndexActivity(Option option, ILogger<BuildWordIndexActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public async Task Build(CancellationToken token)
        {
            _logger.LogInformation($"{nameof(Build)}");

            var context = new Context();
            await RunBuild(context, token);

            UpdateDirectoryToObjFolder(context);
        }

        private async Task RunBuild(Context context, CancellationToken token)
        {
            ActionBlock<string> activities = new ActionBlock<string>(x => BuildIndex(context, x, token), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
            string buildFolder = ArticleConstants.Folders.GetPackageFolder(_option.BuildFolder!);

            string[] specFilePaths = Directory.GetFiles(buildFolder, "*.*", SearchOption.AllDirectories);

            foreach (var filePath in specFilePaths)
            {
                await activities.SendAsync(filePath);
            }

            activities.Complete();
            await activities.Completion;
        }

        private void BuildIndex(Context context, string packageFile, CancellationToken token)
        {
            _logger.LogInformation($"{nameof(BuildIndex)}: packageFile={packageFile}");

            byte[] bytes = File.ReadAllBytes(packageFile);
            ArticlePayload articlePayload = bytes.ToArticlePayload((ArticleId)"id");

            IReadOnlyList<WordCount> wordCounts = new ArticlePackageIndexBuilder().Build(articlePayload);

            ArticleManifest articleManifest = articlePayload.ReadManifest();

            var articleIndex = new ArticleIndex
            {
                ArticleId = articleManifest.ArticleId,
                WordIndexes = wordCounts,
            };

            context.Queue.Enqueue(articleIndex);
        }

        private void UpdateDirectoryToObjFolder(Context context)
        {
            _logger.LogInformation($"{nameof(UpdateDirectoryToObjFolder)}: Updating directory");

            string directoryPath = ArticleConstants.Files.GetDirectoryFile(_option.BuildFolder!);

            ArticleDirectory articleDirectory = new ArticleDirectoryFile(directoryPath).Read();

            articleDirectory = articleDirectory with { Indexes = context.Queue.ToList() };

            articleDirectory.WriteToFile(directoryPath);
        }

        private record Context
        {
            public ConcurrentQueue<ArticleIndex> Queue { get; } = new ConcurrentQueue<ArticleIndex>();
        }
    }
}