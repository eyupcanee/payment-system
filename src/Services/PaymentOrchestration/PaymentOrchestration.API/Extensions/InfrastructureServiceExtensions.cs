using Infrastructure.HttpClientHandlers;

namespace PaymentOrchestration.API.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<CorrelationIdDelegatingHandler>();

        services.AddHttpClient("OtherServices")
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
        
        
        return services;
    }
}