using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Logs;
using System.Net;
using System.Text.Json;

namespace SharedLibrary.Middleware
{
    public class GlobalExceptions(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Please try again ! An Internal Server Error Occured. We are unable to reach..";
            int statusCode = (int)(HttpStatusCode.InternalServerError);
            string title = "Error";

            try
            {
                await next(context);
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests. Please try again later.";
                    statusCode = StatusCodes.Status429TooManyRequests;
                    title = "Too Many Requests";
                    await ModifyHeader(context, message, statusCode, title);
                }
                else if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    message = "The requested resource was not found.";
                    statusCode = StatusCodes.Status404NotFound;
                    title = "Not Found";
                    await ModifyHeader(context, message, statusCode, title);

                }
                else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    message = "You are not authorized to access this resource.";
                    statusCode = StatusCodes.Status401Unauthorized;
                    title = "Unauthorized";
                    await ModifyHeader(context, message, statusCode, title);

                }
                else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    message = "Access to this resource is forbidden.";
                    statusCode = StatusCodes.Status403Forbidden;
                    title = "Forbidden";
                    await ModifyHeader(context, message, statusCode, title);
                    await ModifyHeader(context, message, statusCode, title);

                }
            }
            catch (Exception ex)
            { 
                LogsException.LogException(ex);

                if(ex is TaskCanceledException ||ex is OperationCanceledException || ex is TimeoutException)
                {
                    message = "The request was canceled. Time out ...........";
                    statusCode = StatusCodes.Status499ClientClosedRequest;
                    title = "Client Closed Request";
                }
                await ModifyHeader(context, message, statusCode, title);
            }
        }

        private async static Task ModifyHeader(HttpContext context, string message, int statusCode, string title)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
            return;
        }
    }
}
