using System.Collections.Generic;

namespace nBlog.sdk.Model
{
    public class PingLogs
    {
        public int Count { get; set; }
        public IList<string>? Messages { get; set; }
        public string? Version { get; set; }
    }
}