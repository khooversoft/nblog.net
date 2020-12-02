using Microsoft.Extensions.Logging;
using nBlog.sdk.Model;
using nBlog.sdk.Store;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Actor.Host;
using Toolbox.Tools;

namespace nBlog.sdk.Actors
{
    public class ArticleStoreService : IArticleStoreService
    {
        public IActorHost? _actorHost;
        private readonly IArticleStore _articleStore;
        private readonly ILogger<ArticleStoreService> _logger;

        public ArticleStoreService(IActorHost actorHost, IArticleStore articleStore, ILogger<ArticleStoreService> logger)
        {
            _actorHost = actorHost;
            _articleStore = articleStore;
            _logger = logger;
        }

        public async Task<ArticlePayload?> Get(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            var actorKey = new ActorKey((string)id);
            _logger.LogTrace($"{nameof(Get)}: actorKey={actorKey}, id={id.Id}");

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>(actorKey);
            return await actor.Get(token);
        }

        public async Task Set(ArticlePayload record, CancellationToken token = default)
        {
            record.VerifyNotNull(nameof(record));

            var actorKey = new ActorKey(new ArticleId(record.Id).ToString());
            _logger.LogTrace($"{nameof(Set)}: actorKey={actorKey}, id={record.Id}");

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>(actorKey);
            await actor.Set(record, token);
        }

        public async Task<bool> Delete(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            var actorKey = new ActorKey((string)id);
            _logger.LogTrace($"{nameof(Delete)}: actorKey={actorKey}, id={id.Id}");

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>(actorKey);
            return await actor.Delete(token);
        }

        public async Task<IReadOnlyList<string>> List(QueryParameters queryParameters, CancellationToken token = default) =>
            await _articleStore.List(queryParameters, token);
    }
}