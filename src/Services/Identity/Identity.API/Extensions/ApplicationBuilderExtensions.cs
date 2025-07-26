using System.Globalization;
using Infrastructure.Extensions.Middlewares;
using Serilog;
using Common.Extensions.Middlewares;
using Prometheus;

namespace Identity.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseLocalizationMiddlewares();

        app.UseInfrastructureMiddlewares();
        
        app.UseSecurityMiddlewares();
        
        app.MapControllers();
        
        return app;
    }

    private static WebApplication UseLocalizationMiddlewares(this WebApplication app)
    {
        var supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("tr-TR")
        };

        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        };

        app.UseRequestLocalization(localizationOptions);
        return app;
    }
    
    private static WebApplication UseInfrastructureMiddlewares(this WebApplication app)
    {
        app.UseHttpMetrics(); 
        app.MapMetrics();
        app.UseCorrelationId();
        app.UseSerilogRequestLogging();
        app.UseGlobalExceptionHandlerMiddleware();
      
        return app;
    }
    
    private static WebApplication UseSecurityMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}