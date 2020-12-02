using nBlog.sdk.Model;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;

namespace nBlog.sdk.Actors.Directory
{
    public interface IDirectoryActor : IActor
    {
        Task<bool> Delete(CancellationToken token);
        Task<ArticleDirectory?> Get(CancellationToken token);
        Task Set(ArticleDirectory articleDirectory, CancellationToken token);
    }
}