using System.Collections.Generic;

namespace nBlog.sdk.Model
{
    public record ArticleIndex
    {
        public string ArticleId { get; init; } = null!;

        public IReadOnlyList<WordCount> WordIndexes { get; init; } = null!;
    }
}
