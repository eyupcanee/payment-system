using Common.Extensions.Middlewares;
using Prometheus;
using Serilog;

namespace FraudDetection.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseInfrastructureMiddlewares();
        return app;
    }
    
    private static WebApplication UseInfrastructureMiddlewares(this WebApplication app)
    {
        app.UseHttpMetrics();
        app.MapMetrics();
        app.UseSerilogRequestLogging();
        return app;
    }
}