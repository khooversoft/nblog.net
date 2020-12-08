using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record ArticleDirectory
    {
        public IReadOnlyList<ArticleManifest> Articles { get; init; } = null!;

        public IReadOnlyList<ArticleIndex>? Indexes { get; init; }
    }
}
