using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
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
        private readonly IDataLakeStore _dataLakeStore;
        private readonly ILogger<ArticlePackageActor> _logger;
        private CacheObject<ArticlePayload> _recordCache = new CacheObject<ArticlePayload>(TimeSpan.FromMinutes(10));

        public ArticlePackageActor(IDataLakeStore dataLakeStore, ILogger<ArticlePackageActor> logger)
        {
            _dataLakeStore = dataLakeStore;
            _logger = logger;
        }

        public async Task<ArticlePayload?> Get(CancellationToken token)
        {
            if (_recordCache.TryGetValue(out ArticlePayload value)) return value;

            byte[] fileData = await _dataLakeStore.Read(base.ActorKey.Value, token: token);
            if (fileData == null || fileData.Length == 0) return null;

            ArticlePayload articlePayload = fileData.ToArticlePayload();

            _recordCache.Set(articlePayload);
            return articlePayload;
        }

        public async Task Set(ArticlePayload articlePayload, CancellationToken token)
        {
            articlePayload
                .VerifyNotNull(nameof(articlePayload))
                .VerifyAssert(x => articlePayload.ReadManifest().ArticleId == base.ActorKey.Value, "Id mismatch");

            _logger.LogTrace($"{nameof(Set)}: Writing {articlePayload}");
            await _dataLakeStore.Write(base.ActorKey.Value, articlePayload.ToBytes(), true, token);

            _recordCache.Set(articlePayload);
        }

        public async Task<bool> Delete(CancellationToken token)
        {
            _recordCache.Clear();

            bool state = await _dataLakeStore.Delete(base.ActorKey.Value, token: token);

            await Deactivate();
            return state;
        }
    }
}
