using nBlog.sdk.Actors;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Actor.Host;
using Toolbox.Azure.DataLake;
using Toolbox.Tools;

namespace nBlog.sdk.Services
{
    public class ArticleStore : IArticleStore
    {
        public IActorHost? _actorHost;
        private readonly IDataLakeStore _datalakeStore;

        public ArticleStore(IActorHost actorHost, IDataLakeStore datalakeStore)
        {
            _actorHost = actorHost;
            _datalakeStore = datalakeStore;
        }

        public string Name { get; } = nameof(IArticleStore).ToLowerInvariant();

        public async Task<ArticlePayload?> Get(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>((ActorKey)id);
            return await actor.Get(token);
        }

        public async Task Set(string id, ArticlePayload record, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));
            record.VerifyNotNull(nameof(record));

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>((ActorKey)id);
            await actor.Set(record, token);
        }

        public async Task Delete(string id, CancellationToken token = default)
        {
            id.VerifyNotEmpty(nameof(id));

            IArticlePackageActor actor = _actorHost!.GetActor<IArticlePackageActor>((ActorKey)id);
            await actor.Delete(token);
        }

        //public async Task<string> List(QueryParameters queryParameters, CancellationToken token = default) => await _datalakeStore.Search(queryParameters, x => true, token);

        //public async Task<IReadOnlyList<LinkRecord>> Search(string sqlQuery, IEnumerable<KeyValuePair<string, string>>? parameters = null, CancellationToken token = default) =>
        //    await _linkContainer.Search.Query(sqlQuery, parameters, token);
    }
}
