using Serilog;
using Infrastructure.Extensions.Middlewares;

namespace PaymentOrchestration.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseInfrastructureMiddlewares();

        app.UseSecurityMiddlewares();
        
        app.MapControllers();
        
        return app;
    }

    private static WebApplication UseInfrastructureMiddlewares(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseCorrelationId();
        return app;
    }
    
    private static WebApplication UseSecurityMiddlewares(this WebApplication app)
    {
        app.UseAuthorization();
        return app;
    }
}