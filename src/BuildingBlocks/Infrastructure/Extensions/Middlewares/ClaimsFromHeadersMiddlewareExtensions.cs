using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Extensions.Middlewares;

public static class ClaimsFromHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseClaimsFromHeadersMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClaimsFromHeadersMiddleware>();
    }
}