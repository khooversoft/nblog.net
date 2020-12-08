using Microsoft.Extensions.Logging;
using nBlog.sdk.Model;
using nBlog.sdk.Store;
using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Tools;

namespace nBlog.sdk.Actors.Directory
{
    public class DirectoryActor : ActorBase, IDirectoryActor
    {
        private readonly IDirectoryStore _directoryStore;
        private readonly ILogger<DirectoryActor> _logger;
        private CacheObject<ArticleDirectory> _cache = new CacheObject<ArticleDirectory>(TimeSpan.FromMinutes(10));

        public DirectoryActor(IDirectoryStore directoryStore, ILogger<DirectoryActor> logger)
        {
            _directoryStore = directoryStore;
            _logger = logger;
        }

        public async Task<ArticleDirectory?> Get(CancellationToken token)
        {
            if (_cache.TryGetValue(out ArticleDirectory? value)) return value;

            _logger.LogTrace($"{nameof(Get)}: actorKey={ActorKey}");
            ArticleDirectory? acticleDirectory = await _directoryStore.Get(token);

            if (acticleDirectory == null) return null;

            _logger.LogInformation($"{nameof(DirectoryActor)} *** actorKey={base.ActorKey}, articleDirectory={acticleDirectory}");

            _cache.Set(acticleDirectory);
            return acticleDirectory;
        }

        public async Task Set(ArticleDirectory articleDirectory, CancellationToken token)
        {
            articleDirectory.VerifyNotNull(nameof(articleDirectory));

            _logger.LogTrace($"{nameof(Set)}: Writing");
            await _directoryStore.Set(articleDirectory, token);

            _cache.Set(articleDirectory);
        }

        public async Task<bool> Delete(CancellationToken token)
        {
            _cache.Clear();

            _logger.LogTrace($"{nameof(Delete)}: actorKey={ActorKey}");
            bool state = await _directoryStore.Delete(token);

            await Deactivate();
            return state;
        }
    }
}