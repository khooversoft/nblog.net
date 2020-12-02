using Microsoft.Extensions.Logging;
using nBlog.sdk.Client;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace NBlog.Server.Services
{
    public class DirectoryService
    {
        private readonly IDirectoryClient _directoryClient;
        private readonly ILogger<DirectoryService> _logger;
        private readonly CacheObject<ArticleDirectory?> _cache = new CacheObject<ArticleDirectory?>(TimeSpan.FromMinutes(5));

        public DirectoryService(IDirectoryClient directoryClient, ILogger<DirectoryService> logger)
        {
            _directoryClient = directoryClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<ArticleManifest>> ByDate(int index, int count)
        {
            ArticleDirectory? articleDirectory = await GetDirectory();

            return (articleDirectory?.Articles ?? Array.Empty<ArticleManifest>())
                .OrderByDescending(x => x.Date)
                .Skip(index)
                .Take(count)
                .ToArray();
        }

        public async Task<IReadOnlyList<string>> GetTags()
        {
            ArticleDirectory? articleDirectory = await GetDirectory();

            return (articleDirectory?.Articles ?? Array.Empty<ArticleManifest>())
                .SelectMany(x => x.Tags ?? Array.Empty<string>())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        public async Task<IReadOnlyList<ArticleManifest>> GetByTag(string tag, int index, int count)
        {
            ArticleDirectory? articleDirectory = await GetDirectory();

            return (articleDirectory?.Articles ?? Array.Empty<ArticleManifest>())
                .Where(x => (x.Tags ?? Array.Empty<string>()).Contains(tag, StringComparer.OrdinalIgnoreCase))
                .OrderByDescending(x => x.Date)
                .Skip(index)
                .Take(count)
                .ToArray();
        }

        private async Task<ArticleDirectory?> GetDirectory()
        {
            ArticleDirectory? subject;

            if (_cache.TryGetValue(out subject)) return subject;

            _logger.LogTrace($"{nameof(GetDirectory)} - fetching directory");

            subject = await _directoryClient.Get();
            _cache.Set(subject);

            return subject;
        }
    }
}
