using Identity.Domain.Entities;

namespace Identity.Application.Security.Abstract;

public interface ITokenService
{
    string GenerateJwtToken(AppUser user, List<string> permissions);
}