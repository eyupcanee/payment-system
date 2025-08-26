namespace Infrastructure.Cache.Abstract;

public interface ITokenBlacklistService
{
    Task AddToBlacklistAsync(string token,TimeSpan expirationTime);
    Task<bool> IsBlacklistedAsync(string token);
}