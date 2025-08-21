using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Middlewares;

public class ClaimsFromHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ClaimsFromHeadersMiddleware> _logger;
    
    public ClaimsFromHeadersMiddleware(RequestDelegate next, ILogger<ClaimsFromHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogWarning("--- GELEN HEADER'LAR ---");
        foreach (var header in context.Request.Headers)
        {
            _logger.LogWarning($"{header.Key}: {header.Value}");
        }
        _logger.LogWarning("------------------------");
        
        if (context.Request.Headers.TryGetValue("X-User-Id", out var userId) && !string.IsNullOrEmpty(userId))
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

            if (context.Request.Headers.TryGetValue("X-User-Email", out var email) && !string.IsNullOrEmpty(email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
            }
            
            if (context.Request.Headers.TryGetValue("X-Jti", out var jti) && !string.IsNullOrEmpty(jti))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()));
            }
            
            if (context.Request.Headers.TryGetValue("X-Permissions", out var permissionsHeader))
            {
                var permissionsArray = permissionsHeader.ToArray();
                
                foreach (var permissionValue in permissionsArray)
                {
                    if (!string.IsNullOrEmpty(permissionValue))
                    {
                        var permissions = permissionValue.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrEmpty(p));
                        
                        claims.AddRange(permissions.Select(p => new Claim("permissions", p)));
                    }
                }
                
                var addedPermissions = claims.Where(c => c.Type == "permissions").Select(c => c.Value).ToList();
                _logger.LogDebug($"Permissions eklendi: {string.Join(", ", addedPermissions)}");
            }
            
            var identity = new ClaimsIdentity(claims, "HeaderBasedAuthentication");
            context.User = new ClaimsPrincipal(identity);
            
            _logger.LogDebug($"User oluşturuldu. Claims sayısı: {claims.Count}, IsAuthenticated: {context.User.Identity?.IsAuthenticated}");
        }
        else
        {
            _logger.LogWarning("X-User-Id header'ı bulunamadı veya boş");
        }

        await _next(context);
    }
}