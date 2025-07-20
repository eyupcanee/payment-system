using Common.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions.Middlewares;

public static class GlobalExceptionHandlerMiddlewareExtension
{
    public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}