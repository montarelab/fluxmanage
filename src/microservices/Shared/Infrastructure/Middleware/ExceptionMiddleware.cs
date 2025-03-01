using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
            throw;
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var errorResponse = new ErrorResponse
        {
            StatusCode = 400,
            Message = exception.Message
        };

        await errorResponse.ExecuteAsync(context);
    }
}