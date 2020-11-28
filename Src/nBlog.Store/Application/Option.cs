﻿using Toolbox.Azure.DataLake.Model;
using Toolbox.Model;
using Toolbox.Services;

namespace nBlog.Store.Application
{
    public record Option
    {
        public string Environment { get; init; } = null!;

        public bool Help { get; init; }

        public RunEnvironment RunEnvironment { get; init; }

        public ISecretFilter SecretFilter { get; init; } = null!;

        public string? SecretId { get; init; }

        public DataLakeStoreOption Store { get; init; } = null!;
    }
}