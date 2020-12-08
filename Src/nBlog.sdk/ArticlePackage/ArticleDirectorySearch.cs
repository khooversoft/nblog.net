using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public class ArticleDirectorySearch
    {
        private readonly ArticleDirectory _articleDirectory;

        public ArticleDirectorySearch(ArticleDirectory articleDirectory)
        {
            articleDirectory.VerifyNotNull(nameof(articleDirectory));

            _articleDirectory = articleDirectory;
        }

        public IReadOnlyList<ArticleId> Search(string line)
        {
            if (line.IsEmpty()) return Array.Empty<ArticleId>();

            IReadOnlyList<WordCount> lineWordCounts = new ParseWord().Parse(line);

            var flattenDirectory = (_articleDirectory.Indexes ?? Array.Empty<ArticleIndex>())
                .SelectMany(x => x.WordIndexes, (o, i) => (id: o.ArticleId, wordCount: i))
                .ToList();

            var joinToLine = flattenDirectory
                .Join(lineWordCounts, x => x.wordCount.Word, x => x.Word, (flatten, lineWordCounts) => (flatten, lineWordCounts), StringComparer.OrdinalIgnoreCase)
                .ToList();

            var selectHighWordCount = joinToLine
                .OrderByDescending(x => x.lineWordCounts.Count)
                .Select(x => x.flatten.id)
                .Distinct()
                .Select(x => (ArticleId)x)
                .Take(10)
                .ToList();

            return selectHighWordCount;
        }
    }
}
