using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using nBlog.sdk.Store;
using nBlog.Store.Application;
using System.Threading.Tasks;

namespace nBlog.Store.Middleware
{
    internal class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Option _option;

        public ApiKeyMiddleware(RequestDelegate next, Option option)
        {
            _next = next;
            _option = option;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(StoreConstants.ApiKeyName, out StringValues extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync($"{nameof(StoreConstants.ApiKeyName)} was not provided.");
                return;
            }

            if (_option.ApiKey != extractedApiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}