using System;

namespace nBlog.sdk.Model
{
    public record ContactRequest
    {
        public Guid RequestId { get; init; } = Guid.NewGuid();

        public string Name { get; init; } = null!;

        public string Email { get; init; } = null!;

        public string? Subject { get; init; }

        public string Message { get; init; } = null!;

        public bool RequestResume { get; init; }
    }
}