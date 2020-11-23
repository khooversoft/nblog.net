using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Model;

namespace nBlog.sdk.Model
{
    public record ArticleSpec
    {
        public string PackageFile { get; init; } = null!;

        public ArticleManifest Manifest { get; init; } = null!;

        public IList<CopyTo> Copy { get; set; } = null!;
    }
}
