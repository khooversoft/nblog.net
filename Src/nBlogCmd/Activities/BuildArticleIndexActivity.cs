using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Client;
using nBlog.sdk.Model;
using nBlogCmd.Application;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Tools;

namespace nBlogCmd.Activities
{
    internal class BuildArticleIndexActivity
    {
        private readonly ILogger<BuildArticleIndexActivity> _logger;
        private readonly Option _option;

        public BuildArticleIndexActivity(Option option, ILogger<BuildArticleIndexActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public async Task Build(CancellationToken token)
        {
            _logger.LogInformation($"{nameof(Build)} - Build");

            Context context = new();
            await ScanManifest(context, token);

            _logger.LogInformation($"{nameof(Build)}: Count={context.Queue.Count}");
            var directory = new ArticleDirectory
            {
                Articles = context.Queue.ToList(),
            };

            WriteDirectoryToObjFolder(directory);
        }

        private async Task ScanManifest(Context context, CancellationToken token)
        {
            ActionBlock<string> activities = new ActionBlock<string>(x => ReadManifest(context, x, token), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            string objFolder = ArticleConstants.Folders.GetObjFolder(_option.BuildFolder!);

            string[] specFilePaths = Directory.GetFiles(objFolder, "*.json", SearchOption.AllDirectories);
            _logger.LogInformation($"{nameof(ScanManifest)}: ObjFolder={objFolder}, Count={specFilePaths.Length}");

            foreach (var filePath in specFilePaths)
            {
                await activities.SendAsync(filePath);
            }

            activities.Complete();
            await activities.Completion;
        }

        private void ReadManifest(Context context, string specFilePath, CancellationToken token)
        {
            _logger.LogInformation($"{nameof(ReadManifest)}: {specFilePath}");

            ArticleManifest articleManifest = new ArticleManifestFile(specFilePath).Read();
            context.Queue.Enqueue(articleManifest);
        }

        private void WriteDirectoryToObjFolder(ArticleDirectory directory)
        {
            string json = Json.Default.Serialize(directory);
            string directoryPath = ArticleConstants.Files.GetDirectoryFile(_option.BuildFolder!);

            File.WriteAllText(directoryPath, json);
        }

        private record Context
        {
            public ConcurrentQueue<ArticleManifest> Queue { get; } = new ConcurrentQueue<ArticleManifest>();
        }
    }
}