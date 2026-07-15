using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core_Layer.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            // LogError = Error level → this WILL be written to SQL ✅

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Something went wrong. Please try again later."
            });
        }
    }
}