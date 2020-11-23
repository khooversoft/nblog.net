using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record ArticleManifest
    {
        public string ArticleId { get; init; } = null!;

        public string PackageVersion { get; init; } = "1.0.0.0";

        public string Title { get; init; } = null!;

        public string? Author { get; init; }

        public DateTime Date { get; init; }

        public IList<string>? Tags { get; init; }
    }
}
