using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record KeyValue
    {
        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; init; }

        public string? Value { get; init; }
    }
}
