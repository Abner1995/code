using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Todo.Core.Exceptions;

namespace Todo.Core.Middleware;

public class ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (NotFoundException notFound)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFound.Message);
            logger.LogWarning(notFound.Message);
        }
        catch (FoundException found)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsync(found.Message);
            logger.LogWarning(found.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong");
        }
    }
}
