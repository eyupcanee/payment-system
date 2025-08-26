using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Common.Contracts.Responses;
using Common.Resources;
using Infrastructure.Cache.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Infrastructure.Middlewares;

public class TokenBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITokenBlacklistService blacklistService,IStringLocalizer<SharedResource> localizer)
    {
        var jti = context.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;


        if (!string.IsNullOrEmpty(jti) && await blacklistService.IsBlacklistedAsync(jti))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 401;
            
            var message = localizer["Invalid_Or_Token_Blacklisted"];
            
            var response = ApiResponse<object>.FailResponse(message,context.Response.StatusCode);

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(jsonResponse);
            return;
        }
        
        await _next(context);
    }
}