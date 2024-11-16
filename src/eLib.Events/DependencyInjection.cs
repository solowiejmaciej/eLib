using System.Reflection;
using eLib.Events.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eLib.Events;

public static class ServiceCollectionExtensions
{
    public static void AddPublishing(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IEventPublisher, EventPublisher>();
        ConfigureMassTransit(services, configuration, null);
    }

    public static void AddConsuming(this IServiceCollection services, ConfigurationManager configuration, Assembly assemblyContainingConsumers)
    {
        var consumerTypes = assemblyContainingConsumers.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
            .ToArray();

        ConfigureMassTransit(services, configuration, consumerTypes);
    }

    private static void ConfigureMassTransit(IServiceCollection services, ConfigurationManager configuration, Type[]? consumerTypes)
    {
        var rabbitMqSettings = configuration.GetSection("RabbitMQ");
        var hostname = rabbitMqSettings.GetSection("HostName").Value;
        var username = rabbitMqSettings.GetSection("Username").Value;
        var password = rabbitMqSettings.GetSection("Password").Value;

        services.AddMassTransit(x =>
        {
            // Rejestracja konsumentów, jeśli są podane
            if (consumerTypes != null)
            {
                foreach (var consumerType in consumerTypes)
                {
                    x.AddConsumer(consumerType);
                }
            }

            // Konfiguracja transportu RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(hostname, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                // Konfiguracja endpointów, jeśli są konsumenci
                if (consumerTypes != null)
                {
                    cfg.ConfigureEndpoints(context);
                }
            });
        });
    }
}
