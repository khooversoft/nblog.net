using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Store
{
    public interface IContactRequestStore
    {
        Task<bool> Delete(Guid id, CancellationToken token = default);
        Task<ContactRequest?> Get(Guid id, CancellationToken token = default);
        Task<IReadOnlyList<string>> List(QueryParameters queryParameters, CancellationToken token = default);
        Task Set(ContactRequest contractRequest, CancellationToken token = default);
    }
}