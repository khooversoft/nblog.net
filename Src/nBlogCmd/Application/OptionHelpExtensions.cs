using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;

namespace nBlogCmd.Application
{
    internal static class OptionHelpExtensions
    {
        public static IReadOnlyList<string> GetHelp(this Option _)
        {
            return new[]
            {
                "nBlog CLI - Build Utility",
                "",
                "Help                  : Display help",
                "",
                "Upload a Article Package(s) to storage",
                "  Upload              : Upload command",
                "  PackageFile={file}  : Article Package to upload",
                "",
                "Build ML Package.",
                "  Build               : Build command",
                "  SpecFile={file}     : ML Package specification file (used for building and uploading)",
                "",
                "",
                "Configuration for BlobStorage",
                "  SecretId={secretId}                       : Use .NET Core configuration secret json file.  SecretId indicates which secret file to use.",
                "",
                "  Store:ContainerName={container name}      : Azure Blob Storage container name (required)",
                "  Store:AccountName={accountName}           : Azure Blob Storage account name (required)",
                "  Store:AccountKey={accountKey}             : Azure Blob Storage account key (required)",
            };
        }

        public static void DumpConfigurations(this Option option, ILogger logger)
        {
            const int maxWidth = 80;

            string line = option.GetConfigValues()
                .Prepend(new string('=', maxWidth))
                .Prepend("Current configurations")
                .Aggregate(string.Empty, (a, x) => a += option.SecretFilter.FilterSecrets(x) + Environment.NewLine);

            logger.LogInformation(line);
        }
    }
}
