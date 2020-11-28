using nBlog.sdk.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Services
{
    public interface IArticleStoreService
    {
        Task<bool> Delete(string id, CancellationToken token = default);
        Task<ArticlePayload?> Get(string id, CancellationToken token = default);
        Task<IReadOnlyList<string>> List(QueryParameters queryParameters, CancellationToken token = default);
        Task Set(ArticlePayload record, CancellationToken token = default);
    }
}