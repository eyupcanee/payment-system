using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Application.Security.Abstract;
using Identity.Application.Services.Abstract;
using Identity.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Security.Concrete;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService( IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(AppUser user, List<string> permissions)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permissions", permission));
        }
        
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddDays(1);
        
        var token = new JwtSecurityToken(jwtSettings["Issuer"], jwtSettings["Audience"], claims, expires: expires, signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}