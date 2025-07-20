using Infrastructure.Extensions.Middlewares;
using Serilog;
using Common.Extensions.Middlewares;

namespace Identity.API.Extensions;

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
        app.UseGlobalExceptionHandlerMiddleware();
        app.UseCorrelationId();
        return app;
    }
    
    private static WebApplication UseSecurityMiddlewares(this WebApplication app)
    {
        app.UseAuthorization();
        return app;
    }
}