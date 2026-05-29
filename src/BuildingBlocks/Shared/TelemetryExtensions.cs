using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Shared;

public static class TelemetryExtensions
{
    public static IServiceCollection AddEcommerceTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(t => t
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(options =>
                    options.Endpoint = new Uri(
                        configuration.GetValue<string>("OpenTelemetry:Endpoint") ?? "http://localhost:4317")));

        return services;
    }
}
