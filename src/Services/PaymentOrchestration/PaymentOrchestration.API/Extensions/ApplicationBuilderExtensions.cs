using System.Globalization;
using Common.Extensions.Middlewares;
using Serilog;
using Infrastructure.Extensions.Middlewares;
using Prometheus;

namespace PaymentOrchestration.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseCustomMiddlewares(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseRouting();

        // ✅ Authentication & Authorization sırası burada olmalı
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseLocalizationMiddlewares();
        app.UseInfrastructureMiddlewares();
        app.UseSecurityMiddlewares(); // Buradaki UseAuthorization kaldırıldı!

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
        app.UseSerilogRequestLogging();
        app.UseCorrelationId();
        app.UseGlobalExceptionHandlerMiddleware();
        return app;
    }
    
    private static WebApplication UseSecurityMiddlewares(this WebApplication app)
    {
        app.UseClaimsFromHeadersMiddleware();
        return app;
    }
}