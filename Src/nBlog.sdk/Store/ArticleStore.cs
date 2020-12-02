using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Azure.DataLake;
using Toolbox.Tools;

namespace nBlog.sdk.Store
{
    public class ArticleStore : IArticleStore
    {
        private readonly IDataLakeStore _dataLakeStore;
        private readonly ILogger<ArticleStore> _logger;

        public ArticleStore(IDataLakeStore dataLakeStore, ILogger<ArticleStore> logger)
        {
            _dataLakeStore = dataLakeStore;
            _logger = logger;
        }

        public async Task<bool> Delete(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            return await _dataLakeStore.Delete(ToFullFileName((string)id), token: token);
        }

        public async Task<ArticlePayload?> Get(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));

            byte[] fileData = await _dataLakeStore.Read(ToFullFileName((string)id), token: token);
            if (fileData == null || fileData.Length == 0) return null;

            return fileData.ToArticlePayload();
        }

        public async Task<IReadOnlyList<string>> List(QueryParameters queryParameters, CancellationToken token = default) =>
            (await _dataLakeStore.Search("/", x => x.IsDirectory == false, true, token))
                .Select(x => x.Name)
                .Skip(queryParameters.Index)
                .Take(queryParameters.Count)
                .ToList();

        public async Task Set(ArticlePayload articlePayload, CancellationToken token = default)
        {
            articlePayload.VerifyNotNull(nameof(articlePayload));

            _logger.LogTrace($"{nameof(Set)}: Writing {articlePayload.Id}");
            await _dataLakeStore.Write(ToFullFileName(articlePayload.Id), articlePayload.ToBytes(), true, token);
        }

        private static string ToFullFileName(string id) => id + ".articlePackage";
    }
}