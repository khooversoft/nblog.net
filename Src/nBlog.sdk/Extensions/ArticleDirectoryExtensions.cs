using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolbox.Tools;

namespace nBlog.sdk.Extensions
{
    public static class ArticleDirectoryExtensions
    {
        public static IReadOnlyList<ArticleManifest> GetLatest(this ArticleDirectory subject) => subject.Articles
            .OrderByDescending(x => x.Date)
            .ToList();

        public static IReadOnlyList<ArticleManifest> Get(this ArticleDirectory subject, params ArticleArea[] articleAreas) => subject.Articles
            .Select(x => (subject: x, ArticleId: (ArticleId)x.ArticleId))
            .Where(x => articleAreas.Any(y => x.ArticleId.IsArticleArea(y)))
            .Select(x => x.subject)
            .ToArray();

        public static IReadOnlyList<string> GetReference(this ArticleDirectory subject) => subject.Articles
            .SelectMany(x => x.GetReferences())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();

        public static IReadOnlyList<ArticleManifest> GetByTag(this ArticleDirectory subject, string tag) => subject.Articles
            .Where(x => x.GetReferences().Contains(tag, StringComparer.OrdinalIgnoreCase))
            .ToList();

        public static void Verify(this ArticleDirectory subject)
        {
            subject.VerifyNotNull(nameof(subject));
            subject.Articles.VerifyNotNull(nameof(subject));
        }

        public static void WriteToFile(this ArticleDirectory subject, string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            subject.Verify();

            var json = Json.Default.SerializeFormat(subject);
            File.WriteAllText(filePath, json);
        }

        public static ArticleDirectory ToArticleDirectory(this byte[] subject)
        {
            subject.VerifyAssert(x => x?.Length > 0, $"{nameof(subject)} is empty");

            var json = Encoding.UTF8.GetString(subject);
            return Json.Default.Deserialize<ArticleDirectory>(json).VerifyNotNull($"Invalid {nameof(ArticleDirectory)}");
        }

        public static byte[] ToBytes(this ArticleDirectory subject)
        {
            subject.Verify();

            var json = Json.Default.Serialize(subject);
            return Encoding.UTF8.GetBytes(json);
        }

        public static bool IsValid(this ArticleDirectory subject) => subject != null && subject.Articles != null;
    }
}