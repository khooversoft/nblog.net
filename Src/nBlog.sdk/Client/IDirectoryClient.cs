using nBlog.sdk.Model;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Client
{
    public interface IDirectoryClient
    {
        Task<bool> Delete(CancellationToken token = default);
        Task<ArticleDirectory?> Get(CancellationToken token = default);
        Task Set(ArticleDirectory articleDirectory, CancellationToken token = default);
    }
}