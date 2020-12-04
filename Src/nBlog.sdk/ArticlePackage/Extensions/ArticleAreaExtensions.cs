using System;
using System.Linq;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage.Extensions
{
    public static class ArticleAreaExtensions
    {
        public static ArticleArea ToArticleArea(this string subject)
        {
            ArticleArea? articleArea = subject.ToArticleAreaOrDefault();

            if( articleArea == null) throw new ArgumentException($"Invalid area {subject}");
            return articleArea.Value;
        }

        public static ArticleArea? ToArticleAreaOrDefault(this string subject)
        {
            if (!Enum.TryParse(subject, true, out ArticleArea result)) return null;
            return result;
        }
    }
}
