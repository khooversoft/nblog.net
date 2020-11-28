using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
using nBlog.sdk.Services;
using nBlog.sdk.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Actor;
using Toolbox.Azure.DataLake;
using Toolbox.Tools;

namespace nBlog.sdk.Actors
{
    public class ArticlePackageActor : ActorBase, IArticlePackageActor
    {
        private readonly IArticleStore _acticleStore;
        private readonly ILogger<ArticlePackageActor> _logger;
        private CacheObject<ArticlePayload> _cache = new CacheObject<ArticlePayload>(TimeSpan.FromMinutes(10));

        public ArticlePackageActor(IArticleStore acticleStore, ILogger<ArticlePackageActor> logger)
        {
            _acticleStore = acticleStore;
            _logger = logger;
        }

        public async Task<ArticlePayload?> Get(CancellationToken token)
        {
            if (_cache.TryGetValue(out ArticlePayload? value)) return value;

            ArticlePayload? articlePayload = await _acticleStore.Get((ArticleId)base.ActorKey.Value, token: token);

            if (articlePayload == null) return null;

            _cache.Set(articlePayload);
            return articlePayload;
        }

        public async Task Set(ArticlePayload articlePayload, CancellationToken token)
        {
            articlePayload.VerifyNotNull(nameof(articlePayload))
                .VerifyAssert(x => articlePayload.Id.ToLower() == base.ActorKey.Value, $"Id mismatch - id={articlePayload.Id.ToLower()}, actorKey={ base.ActorKey.Value}");

            _logger.LogTrace($"{nameof(Set)}: Writing {articlePayload.Id}");
            await _acticleStore.Set(articlePayload, token);

            _cache.Set(articlePayload);
        }

        public async Task<bool> Delete(CancellationToken token)
        {
            _cache.Clear();

            bool state = await _acticleStore.Delete((ArticleId)base.ActorKey.Value, token: token);

            await Deactivate();
            return state;
        }
    }
}
