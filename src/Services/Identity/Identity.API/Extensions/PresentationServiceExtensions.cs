using Asp.Versioning;
using Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Extensions;

public static class PresentationServiceExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        });
        
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddOpenApi();
        
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