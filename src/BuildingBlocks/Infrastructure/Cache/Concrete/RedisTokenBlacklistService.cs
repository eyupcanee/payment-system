using Infrastructure.Cache.Abstract;
using Infrastructure.Configurations;
using StackExchange.Redis;

namespace Infrastructure.Cache.Concrete;

public class RedisTokenBlacklistService : ITokenBlacklistService
{
    private readonly IDatabase _db;

    public RedisTokenBlacklistService(RedisBlacklistConfigurations configurations)
    {
        var options = new ConfigurationOptions
        {
            EndPoints = { $"{configurations.Host}:{configurations.Port}" },
            Password = configurations.Password,
            AbortOnConnectFail = false
        };
        
        _db = ConnectionMultiplexer.Connect(options).GetDatabase();
    }

    public Task AddToBlacklistAsync(string token, TimeSpan expirationTime) =>
        _db.StringSetAsync($"blacklist:{token}", "1", expirationTime);
    
    public Task<bool> IsBlacklistedAsync(string token) =>
        _db.KeyExistsAsync($"blacklist:{token}");
}