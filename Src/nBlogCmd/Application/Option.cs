using Toolbox.Azure.DataLake.Model;
using Toolbox.Model;
using Toolbox.Services;

namespace nBlogCmd.Application
{
    internal record Option
    {
        public bool Help { get; init; }
        public string? SecretId { get; init; }
        public string? ConfigFile { get; init; }

        public bool Build { get; init; }
        public string? SourceFolder { get; init; }
        public string? BuildFolder { get; init; }

        public bool Upload { get; init; }
        public string BlogStoreUrl { get; init; } = null!;

        public string Environment { get; init; } = null!;
        public RunEnvironment RunEnvironment { get; init; }

        public ISecretFilter SecretFilter { get; init; } = null!;
    }
}