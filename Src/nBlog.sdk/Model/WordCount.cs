namespace nBlog.sdk.Model
{
    public record WordCount
    {
        public string Word { get; init; } = null!;

        public int Count { get; init; }
    }
}
