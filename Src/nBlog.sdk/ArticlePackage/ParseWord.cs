using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;

namespace nBlog.sdk.ArticlePackage
{
    public class ParseWord
    {
        private static readonly HashSet<string> _skipWords = new HashSet<string>
        {
            "and", "it", "you",
        };

        private static readonly char[] _tokenParse = new char[]
        {
            ' ', '!', '"', '(', ')', ',', '-', '.',
            ':', ';', '<', '=', '>', '?',
            '[', '\\', ']', '^', '_', '`',
            '{', '!', '}', '~'
            //' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
            //':', ';', '<', '=', '>', '?', '@',
            //'[', '\\', ']', '^', '_', '`',
            //'{', '!', '}', '~'
        };

        public IReadOnlyList<WordCount> Parse(string line)
        {
            if (line.IsEmpty()) return Array.Empty<WordCount>();

            string cleanedLine = line
                .Select(x => char.IsWhiteSpace(x) || char.IsControl(x) || char.IsSymbol(x) || x > 122 ? ' ' : x)
                .ToArray()
                .AsSpan()
                .ToString();

            string[] tokens = cleanedLine
                .Split(_tokenParse, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !_skipWords.Contains(x, StringComparer.OrdinalIgnoreCase))
                .Where(x => x.Length > 1)
                .ToArray();

            IReadOnlyList<WordCount> wordCounts = tokens
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Select(x => new WordCount { Word = x.Key, Count = x.Count() })
                .ToList();

            return wordCounts;
        }

    }
}
