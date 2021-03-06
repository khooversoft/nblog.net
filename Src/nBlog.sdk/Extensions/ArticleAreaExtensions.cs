﻿using nBlog.sdk.ArticlePackage;
using System;

namespace nBlog.sdk.Extensions
{
    public static class ArticleAreaExtensions
    {
        public static ArticleArea ToArticleArea(this string subject)
        {
            ArticleArea? articleArea = subject.ToArticleAreaOrDefault();

            if (articleArea == null) throw new ArgumentException($"Invalid area {subject}");
            return articleArea.Value;
        }

        public static ArticleArea? ToArticleAreaOrDefault(this string subject)
        {
            if (!Enum.TryParse(subject, true, out ArticleArea result)) return null;
            return result;
        }
    }
}