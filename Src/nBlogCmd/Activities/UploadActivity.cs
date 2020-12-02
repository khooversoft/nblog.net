using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Client;
using nBlog.sdk.Model;
using nBlogCmd.Application;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace nBlogCmd.Activities
{
    internal class UploadActivity
    {
        private readonly IArticleClient _blogClient;
        private readonly ILogger<BuildActivity> _logger;
        private readonly Option _option;

        public UploadActivity(Option option, IArticleClient blogClient, ILogger<BuildActivity> logger)
        {
            _option = option;
            _blogClient = blogClient;
            _logger = logger;
        }

        public async Task Upload(CancellationToken token)
        {
            ActionBlock<string> activities = new ActionBlock<string>(async x => await UploadPackage(x, token), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            string folder = Path.Combine(_option.BuildFolder!, Constants.PackageFolderName);
            string[] specFilePaths = Directory.GetFiles(folder, "*.articlePackage", SearchOption.AllDirectories);

            foreach (var filePath in specFilePaths)
            {
                await activities.SendAsync(filePath);
            }

            activities.Complete();
            await activities.Completion;
        }

        private async Task UploadPackage(string articlePackageFile, CancellationToken token)
        {
            _logger.LogInformation($"Uploading article package {articlePackageFile}");

            ArticlePayload articlePayload = File.ReadAllBytes(articlePackageFile).ToArticlePayload();

            await _blogClient.Set(articlePayload, token);
        }
    }
}