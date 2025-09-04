using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Add this using
using System.Linq; // Add this for LINQ methods

namespace SharedLibrary.Middleware
{
    public class ListenToApiGateway
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ListenToApiGateway> _logger;

        public ListenToApiGateway(RequestDelegate next, ILogger<ListenToApiGateway> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headerKey = "Api-Gateway";
            var singleHeader = context.Request.Headers[headerKey];

            if (singleHeader.FirstOrDefault() is null)
            {
                // CRITICAL LOG: This will tell you if it's missing and list all other headers.
                var allHeaders = string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}"));
                _logger.LogWarning("ListenToApiGateway: Header '{HeaderKey}' is MISSING for path: {Path}. Received headers: [{AllHeaders}]", headerKey, context.Request.Path, allHeaders);

                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync($"Service Unavailable: Missing {headerKey} header.");
                return;
            }
            else
            {
                _logger.LogInformation("ListenToApiGateway: Success! Found header '{HeaderKey}' with value '{HeaderValue}'", headerKey, singleHeader.First());
                await _next(context);
            }
        }
    }
}