using nBlog.sdk.Model;
using System.Collections.Generic;

namespace nBlog.sdk.ArticlePackage
{
    public record ArticlePackageDetails
    {
        public ArticleManifest ArticleManifest { get; init; } = null!;

        public string BasePath { get; init; } = null!;

        public IReadOnlyList<string> Files { get; init; } = null!;
    }
}