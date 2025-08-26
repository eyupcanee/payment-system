using MassTransit;
using PaymentGateway.Application.Consumers;

namespace PaymentGateway.API.Extensions;

public static class PresentationServiceExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProcessBankACommandConsumer>();
            x.AddConsumer<ProcessBankBCommandConsumer>();
            
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

        return services;
    }
}