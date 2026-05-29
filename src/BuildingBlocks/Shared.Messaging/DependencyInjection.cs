using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace BuildingBlocks.Shared.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionFactory>(sp =>
        {
            var config = configuration.GetSection("EventBus");
            return new ConnectionFactory
            {
                HostName = config.GetValue<string>("HostName") ?? "localhost",
                Port = config.GetValue<int>("Port", 5672),
                UserName = config.GetValue<string>("UserName") ?? "guest",
                Password = config.GetValue<string>("Password") ?? "guest",
                VirtualHost = config.GetValue<string>("VirtualHost") ?? "/",
                DispatchConsumersAsync = true
            };
        });

        services.AddSingleton<IEventBus, EventBusRabbitMQ>();
        return services;
    }
}
