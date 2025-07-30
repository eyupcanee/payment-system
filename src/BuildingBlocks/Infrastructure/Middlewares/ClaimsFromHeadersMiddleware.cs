using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middlewares;

public class ClaimsFromHeadersMiddleware
{
    private readonly RequestDelegate _next;
    
    public ClaimsFromHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        System.Console.WriteLine("--- GELEN HEADER'LAR ---");
        foreach (var header in context.Request.Headers)
        {
            System.Console.WriteLine($"{header.Key}: {header.Value}");
        }
        System.Console.WriteLine("------------------------");
        if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

            if (context.Request.Headers.TryGetValue("X-User-Email", out var email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
            }
            
            if (context.Request.Headers.TryGetValue("X-Jti", out var jti))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()));
            }

            if (context.Request.Headers.TryGetValue("X-Permissions", out var permissionsHeader))
            {
                var permissions = permissionsHeader.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                claims.AddRange(permissions.Select(p => new Claim("permissions", p.Trim())));
            }
            
            var identity = new ClaimsIdentity(claims, "Gateway");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}