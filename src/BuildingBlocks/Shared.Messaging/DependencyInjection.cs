using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Shared.Messaging;

public static class MessagingDependencyInjection
{
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                var config = configuration.GetSection("EventBus");
                cfg.Host(config.GetValue<string>("HostName") ?? "localhost", h =>
                {
                    h.Username(config.GetValue<string>("UserName") ?? "guest");
                    h.Password(config.GetValue<string>("Password") ?? "guest");
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddSingleton<IEventBus, EventBusMassTransit>();
        return services;
    }

    public static IServiceCollection AddEventBusWithConsumers(
        this IServiceCollection services,
        IConfiguration configuration,
        params Type[] consumerTypes)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumers(consumerTypes);

            x.UsingRabbitMq((context, cfg) =>
            {
                var config = configuration.GetSection("EventBus");
                cfg.Host(config.GetValue<string>("HostName") ?? "localhost", h =>
                {
                    h.Username(config.GetValue<string>("UserName") ?? "guest");
                    h.Password(config.GetValue<string>("Password") ?? "guest");
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddSingleton<IEventBus, EventBusMassTransit>();
        return services;
    }
}
