using Infrastructure.Cache.Abstract;
using Infrastructure.Cache.Concrete;
using Infrastructure.Configurations;
using Infrastructure.HttpClientHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PaymentOrchestration.Infrastructure;

namespace PaymentOrchestration.API.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(connectionString));
        
        services.AddDbContext<PaymentSagaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PaymentOrchestrationService"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("PaymentOrchestrationService")
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(configuration.GetValue<string>("Jaeger:Uri")!);
                    });
            });

        services.Configure<RedisBlacklistConfigurations>(
            configuration.GetSection("RedisBlackList"));
        services.AddSingleton<ITokenBlacklistService>(provider =>
        {
            var configs = provider.GetRequiredService<IOptions<RedisBlacklistConfigurations>>().Value;
            return new RedisTokenBlacklistService(configs);
        });

        services.AddTransient<CorrelationIdDelegatingHandler>();

        services.AddHttpClient("OtherServices")
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
        
        
        return services;
    }
}