using nBlog.sdk.Model;
using System.Threading;
using System.Threading.Tasks;

namespace nBlog.sdk.Actors.Directory
{
    public interface IDirectoryService
    {
        Task<bool> Delete(CancellationToken token = default);
        Task<ArticleDirectory?> Get(CancellationToken token = default);
        Task Set(ArticleDirectory record, CancellationToken token = default);
    }
}