using Identity.Domain.Entities;

namespace Identity.Application.Services.Abstract;

public interface IIdentityService
{
    public Task<string> LoginAsync(string email, string password);
    public Task<AppUser> RegisterAsync(string email, string password);
    public Task LogoutAsync(string token);
}