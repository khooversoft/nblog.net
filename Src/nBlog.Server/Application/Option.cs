using Toolbox.Azure.DataLake.Model;
using Toolbox.Model;
using Toolbox.Services;

namespace NBlog.Server.Application
{
    internal record Option
    {
        public string? SecretId { get; init; }

        public BlogStoreOption BlogStoreOption { get; init; } = null!;

        public string Environment { get; init; } = null!;
        public RunEnvironment RunEnvironment { get; init; }

        public string? InstrumentationKey { get; init; }

        public ISecretFilter SecretFilter { get; init; } = null!;
    }
}