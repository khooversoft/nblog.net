using Microsoft.Extensions.Logging;
using nBlog.sdk.Model;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlog.sdk.Client
{
    public class BlogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public BlogClient(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ArticlePayload?> Get(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));
            _logger.LogTrace($"{nameof(Get)}: Id={id}");

            try
            {
                return await _httpClient.GetFromJsonAsync<ArticlePayload?>($"api/article/{id.ToBase64()}", token);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{nameof(Get)}: id={id} failed");
                return null;
            }
        }

        public async Task Set(ArticlePayload articlePayload, CancellationToken token = default)
        {
            articlePayload.VerifyNotNull(nameof(articlePayload));

            _logger.LogTrace($"{nameof(Set)}: Id={articlePayload.Id}");

            await _httpClient.PostAsJsonAsync("api/article", articlePayload, token);
        }

        public async Task<bool> Delete(ArticleId id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));
            _logger.LogTrace($"{nameof(Delete)}: Id={id}");

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/article/{id.ToBase64()}", token);

            return response.StatusCode switch
            {
                HttpStatusCode.OK => true,
                HttpStatusCode.NotFound => false,

                _ => throw new HttpRequestException($"Invalid http code={response.StatusCode}"),
            };
        }

        public BatchSetCursor<string> List(QueryParameters queryParameters) => new BatchSetCursor<string>(_httpClient, queryParameters, _logger);
    }
}