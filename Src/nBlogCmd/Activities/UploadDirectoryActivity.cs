using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Client;
using nBlog.sdk.Model;
using nBlogCmd.Application;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nBlogCmd.Activities
{
    internal class UploadDirectoryActivity
    {
        private readonly ILogger<BuildArticleIndexActivity> _logger;
        private readonly Option _option;
        private readonly IDirectoryClient _directoryClient;

        public UploadDirectoryActivity(Option option, IDirectoryClient directoryClient, ILogger<BuildArticleIndexActivity> logger)
        {
            _option = option;
            _directoryClient = directoryClient;
            _logger = logger;
        }

        public async Task Upload(CancellationToken token)
        {
            _logger.LogInformation($"{nameof(Upload)} - Upload to storage");

            string directoryFilePath = ArticleConstants.Files.GetDirectoryFile(_option.BuildFolder!);

            ArticleDirectory articleDirectory = new ArticleDirectoryFile(directoryFilePath).Read();

            _logger.LogInformation($"{nameof(Upload)} - Upload");
            await _directoryClient.Set(articleDirectory);
        }
    }
}