namespace NBlog.Server.Application
{
    public record BlogStoreOption
    {
        public string ApiKey { get; init; } = null!;

        public string Url { get; init; } = null!;
    }
}