using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public record QueryParameters
    {
        public int Index { get; init; } = 0;

        public int Count { get; init; } = 1000;

        public static QueryParameters Default { get; } = new QueryParameters();
    }
}
