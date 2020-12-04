using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record ArticleManifest
    {
        private string _articleId = null!;

        public string ArticleId
        {
            get => _articleId;
            init => _articleId = new ArticleId(value).Id;
        }

        public string PackageVersion { get; init; } = "1.0.0.0";

        public string Title { get; init; } = null!;

        public string? Summary { get; init; }

        public string? Author { get; init; }

        public string? ImageFile { get; set; }

        public DateTime Date { get; init; } = DateTime.Now;

        public IList<string>? Tags { get; init; }

        public IList<string>? Categories { get; init; }
    }
}
