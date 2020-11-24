using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record ArticlePayload
    {
        public string PackagePayload { get; init; } = null!;

        public string Hash { get; set; } = null!;
    }
}
