using Microsoft.Extensions.Logging;
using nBlogCmd.Application;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace nBlogCmd.Activities
{
    internal class ClearBuildFolderActivity
    {
        private readonly ILogger<ClearBuildFolderActivity> _logger;
        private readonly Option _option;

        public ClearBuildFolderActivity(Option option, ILogger<ClearBuildFolderActivity> logger)
        {
            _option = option;
            _logger = logger;
        }

        public Task Clear()
        {
            _option.BuildFolder.VerifyNotEmpty($"{nameof(_option.BuildFolder)} not specified");

            if (Directory.Exists(_option.BuildFolder)) Directory.Delete(_option.BuildFolder, true);

            Directory.CreateDirectory(_option.BuildFolder);

            return Task.CompletedTask;
        }
    }
}