using Microsoft.Extensions.Logging;
using nBlog.sdk.Client;
using nBlog.sdk.Model;
using NBlog.Server.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace NBlog.Server.Services
{
    public class ArticleService
    {
        private readonly IArticleClient _articleClient;
        private readonly ILogger<ArticleService> _logger;
        private readonly ConcurrentDictionary<string, CacheObject<ArticleCache>> _articles = new ConcurrentDictionary<string, CacheObject<ArticleCache>>();

        public ArticleService(IArticleClient articleClient, ILogger<ArticleService> logger)
        {
            _articleClient = articleClient;
            _logger = logger;
        }

        public async Task<ArticleCache?> Get(ArticleId articleId)
        {
            if (_articles.TryGetValue((string)articleId, out CacheObject<ArticleCache>? cache))
            {
                if (cache.TryGetValue(out ArticleCache? cachedInternal)) return cachedInternal;
            }

            ArticlePayload? payload = await _articleClient.Get(articleId);
            if (payload == null) return null;

            var articleCache = new ArticleCache(payload);

            _articles.AddOrUpdate((string)articleId, x => new CacheObject<ArticleCache>(TimeSpan.FromMinutes(5)).Set(articleCache), (k, x) => x.Set(articleCache));

            return articleCache;
        }
    }
}
