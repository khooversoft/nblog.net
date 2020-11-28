using nBlog.sdk.Actors;
using nBlog.sdk.Model;
using nBlog.sdk.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Actor.Host;
using Toolbox.Azure.DataLake;
using Toolbox.Tools;

namespace nBlog.sdk.Services
{
    public class ArticleStoreService : IArticleStoreService
    {
        public IActorHost? _actorHost;
        private readonly IArticleStore _articleStore;

        public ArticleStoreService(IActorHost actorHost, IArticleStore articleStore)
        {
            _actorHost = actorHost;
            _articleStore = articleStore;
        }

        public async Task<ArticlePayload?> Get(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>(new ActorKey((string)id));
            return await actor.Get(token);
        }

        public async Task Set(ArticlePayload record, CancellationToken token = default)
        {
            record.VerifyNotNull(nameof(record));

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>(new ActorKey(record.Id));
            await actor.Set(record, token);
        }

        public async Task<bool> Delete(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>(new ActorKey((string)id));
            return await actor.Delete(token);
        }

        public async Task<IReadOnlyList<string>> List(QueryParameters queryParameters, CancellationToken token = default) =>
            await _articleStore.List(queryParameters, token);
    }
}