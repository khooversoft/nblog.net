using nBlog.sdk.Model;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Services
{
    public interface IArticleStore
    {
        string Name { get; }

        Task Delete(string id, CancellationToken token = default);
        Task<ArticlePayload?> Get(string id, CancellationToken token = default);
        Task Set(string id, ArticlePayload record, CancellationToken token = default);
    }
}