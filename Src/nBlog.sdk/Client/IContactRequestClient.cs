using nBlog.sdk.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Client
{
    public interface IContactRequestClient
    {
        Task<bool> Delete(Guid id, CancellationToken token = default);
        Task<ContactRequest?> Get(Guid id, CancellationToken token = default);
        BatchSetCursor<string> List(QueryParameters queryParameters);
        Task Set(ContactRequest contactRequest, CancellationToken token = default);
    }
}