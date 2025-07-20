using Identity.Domain.Entities;
using Identity.Infrastructure;
using Infrastructure.Cache.Abstract;
using Infrastructure.Cache.Concrete;
using Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Identity.API.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("IdentityService"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("IdentityService")
                    .AddJaegerExporter(o =>
                    {
                        o.AgentHost = configuration["Jaeger:Host"] ?? "localhost";
                        o.AgentPort = int.Parse(configuration["Jaeger:Port"] ?? "6831");
                    });
            });

        services.Configure<RedisBlacklistConfigurations>(
            configuration.GetSection("RedisBlacklist"));
        services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        services.AddSingleton<ITokenBlacklistService>(provider =>
        {
            var configs = provider.GetRequiredService<IOptions<RedisBlacklistConfigurations>>().Value;
            return new RedisTokenBlacklistService(configs);
        });

        return services;
    }
}