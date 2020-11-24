using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nBlog.sdk.Model
{
    public class BatchSet<T>
    {
        public QueryParameters QueryParameters { get; set; } = null!;

        public int NextIndex { get; set; }

        public IReadOnlyList<T> Records { get; set; } = null!;
    }
}
