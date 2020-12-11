namespace nBlogCmd.Application
{
    public record BlogStoreOption
    {
        public string ApiKey { get; init; } = null!;

        public string StoreUrl { get; init; } = null!;
    }
}