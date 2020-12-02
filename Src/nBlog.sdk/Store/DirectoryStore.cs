using Microsoft.Extensions.Logging;
using nBlog.sdk.ArticlePackage;
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
    public class DirectoryStore : IDirectoryStore
    {
        private readonly IDataLakeStore _dataLakeStore;
        private readonly ILogger<DirectoryStore> _logger;

        public DirectoryStore(IDataLakeStore dataLakeStore, ILogger<DirectoryStore> logger)
        {
            _dataLakeStore = dataLakeStore;
            _logger = logger;
        }

        public async Task<bool> Delete(CancellationToken token = default) => await _dataLakeStore.Delete(ArticleConstants.DirectoryFileName, token);

        public async Task<ArticleDirectory?> Get(CancellationToken token = default)
        {
            byte[] fileData = await _dataLakeStore.Read(ArticleConstants.DirectoryFileName, token);
            if (fileData == null || fileData.Length == 0) return null;

            return fileData.ToArticleDirectory();
        }

        public async Task Set(ArticleDirectory articleDirectory, CancellationToken token = default)
        {
            articleDirectory.VerifyNotNull(nameof(articleDirectory));

            _logger.LogTrace($"{nameof(Set)}: Writing");
            await _dataLakeStore.Write(ArticleConstants.DirectoryFileName, articleDirectory.ToBytes(), true, token);
        }
    }
}