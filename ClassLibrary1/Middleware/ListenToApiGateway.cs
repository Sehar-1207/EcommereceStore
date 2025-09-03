using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Middleware
{
    public class ListenToApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var singleHeader = context.Request.Headers["Api-Gateway"];
            if (singleHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service Unavailable: Missing Api-Gateway header.");
                return;
            }
            else
            {
                await next(context);
            }
        }

    }
}
