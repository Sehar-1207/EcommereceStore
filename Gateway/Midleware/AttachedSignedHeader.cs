using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Add this using

namespace Gateway.Midleware
{
    public class AttachedSignedHeader
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AttachedSignedHeader> _logger;

        // Inject the logger
        public AttachedSignedHeader(RequestDelegate next, ILogger<AttachedSignedHeader> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headerKey = "Api-Gateway";
            var headerValue = "Signed";

            // Log BEFORE adding the header
            _logger.LogInformation("AttachedSignedHeader: Attempting to add header '{HeaderKey}: {HeaderValue}' to request for path: {Path}", headerKey, headerValue, context.Request.Path);

            context.Request.Headers[headerKey] = headerValue;

            // Log AFTER to confirm it's in the collection
            _logger.LogInformation("AttachedSignedHeader: Header added. Current value is '{CurrentValue}'", context.Request.Headers[headerKey].FirstOrDefault());

            await _next(context);
        }
    }
}