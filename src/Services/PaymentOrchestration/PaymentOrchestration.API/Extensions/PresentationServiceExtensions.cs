using Asp.Versioning;
using Common.Filters;
using FluentValidation;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using PaymentOrchestration.Application.Interfaces.Repositories;
using PaymentOrchestration.Infrastructure.Repositories;

namespace PaymentOrchestration.API.Extensions;

public static class PresentationServiceExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });

        services.AddOpenApi();

        // 🔹 Dummy authentication scheme tanımlıyoruz
        services.AddAuthentication("HeaderBasedAuthentication")
            .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>("HeaderBasedAuthentication", null);

        services.AddAuthorization();

        // 🔹 Custom policy ve permission handler
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(PaymentOrchestration.Application.AssemblyReference).Assembly));

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        return services;
    }
}
