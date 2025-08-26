using System.Text;
using Asp.Versioning;
using Common.Filters;
using FluentValidation;
using Infrastructure.Authorization;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using PaymentOrchestration.Application.Configuration.Bank;
using PaymentOrchestration.Application.Interfaces.Repositories;
using PaymentOrchestration.Application.Services.Abstract;
using PaymentOrchestration.Application.Services.Concrete;
using PaymentOrchestration.Application.StateMachines;
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
        
        services.Configure<BankSettings>(configuration.GetSection("BankConfigurations"));

        services.AddOpenApi();

   
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Bearer", options =>
            {
                var jwtSettings = configuration.GetSection("Jwt");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"]!)), 
                };
            });

        services.AddAuthorization();
        
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddScoped<IPaymentRequestRepository, PaymentRequestRepository>();
        services.AddScoped<IRoutingService, RoutingService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(PaymentOrchestration.Application.AssemblyReference).Assembly));

        services.AddMassTransit(x =>
        {
            x.AddConsumers(typeof(PaymentOrchestration.Application.AssemblyReference).Assembly);

            x.AddSagaStateMachine<PaymentRequestSagaStateMachine, PaymentRequestSagaState>()
                .InMemoryRepository();
            
            x.AddActivities(typeof(PaymentOrchestration.Application.AssemblyReference).Assembly);
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetSection("RabbitMq")["Host"],"/", h =>
                {
                    h.Username(configuration.GetSection("RabbitMq")["Username"]!);
                    h.Password(configuration.GetSection("RabbitMq")["Password"]!);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        
        });
        
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
