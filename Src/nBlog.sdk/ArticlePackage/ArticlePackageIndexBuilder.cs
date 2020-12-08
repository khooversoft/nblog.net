using nBlog.sdk.Extensions;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public class ArticlePackageIndexBuilder
    {
        private static readonly string[] _documentExtensionsToIndex = new[] { "md" };

        public IReadOnlyList<WordCount> Build(string packageFile)
        {
            packageFile.VerifyNotEmpty(nameof(packageFile));
            packageFile.VerifyAssert(x => File.Exists(x), $"{packageFile} does not exist");

            byte[] bytes = File.ReadAllBytes(packageFile);
            ArticlePayload articlePayload = bytes.ToArticlePayload((ArticleId)"id");

            return Build(articlePayload);
        }

        public IReadOnlyList<WordCount> Build(ArticlePayload articlePayload)
        {
            IReadOnlyList<string> docsToIndex = articlePayload.GetPackageEntries()
                .Where(x =>
                {
                    var extension = Path.GetExtension(x).Replace(".", string.Empty);
                    return _documentExtensionsToIndex.Any(x => string.Equals(x, extension, StringComparison.OrdinalIgnoreCase));
                }).ToList();

            var list = docsToIndex
                .SelectMany(x => IndexDoc(articlePayload, x))
                .ToList();

            return list
                .GroupBy(x => x.Word, StringComparer.OrdinalIgnoreCase)
                .Select(x => new WordCount { Word = x.Key, Count = x.Sum(x => x.Count) })
                .OrderBy(x => x.Word)
                .ToList();
        }

        private IReadOnlyList<WordCount> IndexDoc(ArticlePayload articlePayload, string documentPath)
        {
            byte[] documentBytes = articlePayload.GetPackageItem(documentPath);
            string documentRaw = Encoding.UTF8.GetString(documentBytes.RemoveBOM());

            return new ParseWord().Parse(documentRaw);
        }
    }
}