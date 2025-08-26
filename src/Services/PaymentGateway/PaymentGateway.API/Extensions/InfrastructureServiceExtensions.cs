using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace PaymentGateway.API.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PaymentGatewayService"))
                    .AddAspNetCoreInstrumentation()
                    .AddSource("PaymentGatewayService")
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(configuration.GetValue<string>("Jaeger:Uri")!);
                    });
            });
        
        return services;
    }
}