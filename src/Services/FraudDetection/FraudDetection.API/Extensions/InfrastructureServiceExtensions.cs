using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FraudDetection.API.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("FraudDetectionService"))
                    .AddAspNetCoreInstrumentation()
                    .AddSource("FraudDetectionService")
                    .AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(configuration.GetValue<string>("Jaeger:Uri")!);
                    });
            });
        
        return services;
    }
}