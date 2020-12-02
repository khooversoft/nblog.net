using Microsoft.Extensions.Logging;
using nBlog.sdk.Model;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace nBlog.sdk.Client
{
    public class DirectoryClient : IDirectoryClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DirectoryClient> _logger;

        public DirectoryClient(HttpClient httpClient, ILogger<DirectoryClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> Delete(CancellationToken token = default)
        {
            _logger.LogTrace($"{nameof(Delete)}");

            HttpResponseMessage response = await _httpClient.DeleteAsync("api/directory", token);

            return response.StatusCode switch
            {
                HttpStatusCode.OK => true,
                HttpStatusCode.NotFound => false,

                _ => throw new HttpRequestException($"Invalid http code={response.StatusCode}"),
            };
        }

        public async Task<ArticleDirectory?> Get(CancellationToken token = default)
        {
            try
            {
                _logger.LogTrace($"{nameof(Get)}");
                return await _httpClient.GetFromJsonAsync<ArticleDirectory?>("api/directory", token);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{nameof(Get)} failed");
                return null;
            }
            catch(Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, $"{nameof(Get)} failed");
                return null;
            }
        }

        public async Task Set(ArticleDirectory articleDirectory, CancellationToken token = default)
        {
            articleDirectory.VerifyNotNull(nameof(articleDirectory));

            _logger.LogTrace($"{nameof(Set)}");
            await _httpClient.PostAsJsonAsync("api/directory", articleDirectory, token);
        }
    }
}