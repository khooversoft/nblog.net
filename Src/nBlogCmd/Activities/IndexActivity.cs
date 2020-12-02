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

namespace nBlogCmd.Activities
{
    internal class IndexActivity
    {
        private readonly ILogger<IndexActivity> _logger;
        private readonly Option _option;
        private readonly IDirectoryClient _directoryClient;

        public IndexActivity(Option option, IDirectoryClient directoryClient, ILogger<IndexActivity> logger)
        {
            _option = option;
            _directoryClient = directoryClient;
            _logger = logger;
        }

        public async Task BuildAndUpload(CancellationToken token)
        {
            _logger.LogInformation($"{nameof(BuildAndUpload)} - Build");

            Context context = new();
            await ScanManifest(context, token);

            _logger.LogInformation($"{nameof(BuildAndUpload)}: Count={context.Queue.Count}");
            var directory = new ArticleDirectory
            {
                Articles = context.Queue.ToList(),
            };

            _logger.LogInformation($"{nameof(BuildAndUpload)} - Upload");
            await _directoryClient.Set(directory);
        }

        private async Task ScanManifest(Context context, CancellationToken token)
        {
            ActionBlock<string> activities = new ActionBlock<string>(x => ReadManifest(context, x, token), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            string objFolder = Path.Combine(_option.BuildFolder!, ArticleConstants.ObjFolderName);
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

        private record Context
        {
            public ConcurrentQueue<ArticleManifest> Queue { get; } = new ConcurrentQueue<ArticleManifest>();
        }
    }
}