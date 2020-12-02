using Microsoft.Extensions.Logging;
using nBlog.sdk.Model;
using nBlog.sdk.Store;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Actor.Host;
using Toolbox.Tools;

namespace nBlog.sdk.Actors.Directory
{
    public class DirectoryService : IDirectoryService
    {
        private static readonly ActorKey _actorKey = new ActorKey("master");
        public IActorHost? _actorHost;
        private readonly IDirectoryStore _directoryStore;
        private readonly ILogger<DirectoryService> _logger;

        public DirectoryService(IActorHost actorHost, IDirectoryStore directoryStore, ILogger<DirectoryService> logger)
        {
            _actorHost = actorHost;
            _directoryStore = directoryStore;
            _logger = logger;
        }

        public async Task<ArticleDirectory?> Get(CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Get)}: actorKey={_actorKey}");

            IDirectoryActor actor = _actorHost!.GetActor<IDirectoryActor>(_actorKey);
            return await actor.Get(token);
        }

        public async Task Set(ArticleDirectory record, CancellationToken token = default)
        {
            record.VerifyNotNull(nameof(record));

            _logger.LogTrace($"{nameof(Set)}: actorKey={_actorKey}");

            IDirectoryActor actor = _actorHost!.GetActor<IDirectoryActor>(_actorKey);
            await actor.Set(record, token);
        }

        public async Task<bool> Delete(CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Delete)}: actorKey={_actorKey}");

            IDirectoryActor actor = _actorHost!.GetActor<IDirectoryActor>(_actorKey);
            return await actor.Delete(token);
        }
    }
}