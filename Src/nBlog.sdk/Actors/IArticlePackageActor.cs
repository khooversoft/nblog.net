using nBlog.sdk.Model;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;

namespace nBlog.sdk.Actors
{
    public interface IArticlePackageActor : IActor
    {
        Task<bool> Delete(CancellationToken token);
        Task<ArticlePayload?> Get(CancellationToken token);
        Task Set(ArticlePayload articlePayload, CancellationToken token);
    }
}