using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Azure.DataLake.Model;

namespace nBlogCmd.Application
{
    public record Option
    {
        public bool Help { get; init; }

        public string? ConfigFile { get; init; }

        public string? LogFolder { get; init; }

        public string? SecretId { get; init; }

        public bool Initialize { get; init; }

        public bool Upload { get; init; }

        public string? Folder { get; init; }

        public DataLakeStoreOption Store { get; init; } = null!;
    }
}
