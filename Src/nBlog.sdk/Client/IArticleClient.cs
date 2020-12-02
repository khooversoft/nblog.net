using nBlog.sdk.Model;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Client
{
    public interface IArticleClient
    {
        Task<bool> Delete(ArticleId id, CancellationToken token = default);
        Task<ArticlePayload?> Get(ArticleId id, CancellationToken token = default);
        BatchSetCursor<string> List(QueryParameters queryParameters);
        Task Set(ArticlePayload articlePayload, CancellationToken token = default);
    }
}