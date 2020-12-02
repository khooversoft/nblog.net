using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using System;
using System.Linq;
using Toolbox.Tools;

namespace NBlog.Server.Model
{
    public class ArticleCache
    {
        private ArticleManifest? _articleManifest;
        private MarkdownDoc? _markdownDoc;

        public ArticleCache(ArticlePayload articlePayload)
        {
            articlePayload.VerifyNotNull(nameof(articlePayload));
            articlePayload.Verify();

            ArticlePayload = articlePayload;
        }

        public ArticlePayload ArticlePayload { get; }

        public ArticleManifest GetArticleManifest() => _articleManifest ??= ArticlePayload.ReadManifest();

        public MarkdownDoc GetIndexMd() => _markdownDoc ??= new MarkdownDoc(ArticlePayload.GetPackageItem(GetIndexName()));

        public byte[] GetPackageItem(string path) => ArticlePayload.GetPackageItem(path);

        public string GetIndexName() => GetArticleManifest().ArticleId.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
    }
}