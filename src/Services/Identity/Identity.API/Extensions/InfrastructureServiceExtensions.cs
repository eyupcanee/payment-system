using Identity.Infrastructure;
using Infrastructure.Cache.Abstract;
using Infrastructure.Cache.Concrete;
using Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.API.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddSingleton<ITokenBlacklistService>(provider =>
        {
            var configs = provider.GetRequiredService<IOptions<RedisBlacklistConfigurations>>().Value;
            return new RedisTokenBlacklistService(configs);
        });

        return services;
    }
}