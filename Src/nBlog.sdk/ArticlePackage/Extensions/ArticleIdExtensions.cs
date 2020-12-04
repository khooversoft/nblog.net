using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage.Extensions
{
    public static class ArticleIdExtensions
    {
        public static ArticleArea GetArticleArea(this ArticleId articleId) => articleId
                .VerifyNotNull(nameof(articleId))
                .Id.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()
                .VerifyNotEmpty($"Id {articleId.Id} does not contain area (ex: area/subject)")
                .ToArticleArea();

        public static bool IsArticleArea(this ArticleId articleId, ArticleArea articleArea) => articleId
            .VerifyNotNull(nameof(articleId))
            .GetArticleArea() == articleArea;
    }
}
