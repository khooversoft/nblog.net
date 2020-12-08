using Microsoft.Extensions.Logging;
using nBlog.sdk.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlog.sdk.Client
{
    public class ContactRequestClient : IContactRequestClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContactRequestClient> _logger;

        public ContactRequestClient(HttpClient httpClient, ILogger<ContactRequestClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ContactRequest?> Get(Guid id, CancellationToken token = default)
        {

            _logger.LogTrace($"{nameof(Get)}: Id={id}");

            try
            {
                return await _httpClient.GetFromJsonAsync<ContactRequest?>($"api/contactRequest/{id}", token);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{nameof(Get)}: id={id} failed");
                return null;
            }
        }

        public async Task Set(ContactRequest contactRequest, CancellationToken token = default)
        {
            contactRequest.VerifyNotNull(nameof(contactRequest));

            _logger.LogTrace($"{nameof(Set)}: Id={contactRequest.RequestId}");

            await _httpClient.PostAsJsonAsync("api/contactRequest", contactRequest, token);
        }

        public async Task<bool> Delete(Guid id, CancellationToken token = default)
        {
            id.VerifyNotNull(nameof(id));
            _logger.LogTrace($"{nameof(Delete)}: Id={id}");

            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/contactRequest/{id}", token);

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