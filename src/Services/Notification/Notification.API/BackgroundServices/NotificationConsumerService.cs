using BuildingBlocks.Shared.Messaging;
using Notification.API.Services;

namespace Notification.API.BackgroundServices;

// For simplicity, using a shared IntegrationEvent class
// In production, use a shared contracts NuGet package
public record NotificationOrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}

public class NotificationOrderCreatedEventHandler : IIntegrationEventHandler<NotificationOrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationOrderCreatedEventHandler> _logger;

    public NotificationOrderCreatedEventHandler(IEmailService emailService, ILogger<NotificationOrderCreatedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(NotificationOrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing notification for order {OrderNumber}", @event.OrderNumber);
        await _emailService.SendOrderConfirmationAsync("customer@example.com", @event.OrderNumber, @event.TotalAmount, cancellationToken);
    }
}

public class NotificationConsumerService : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationConsumerService> _logger;

    public NotificationConsumerService(
        IEventBus eventBus,
        IServiceProvider serviceProvider,
        ILogger<NotificationConsumerService> logger)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification consumer service starting...");

        var handler = new NotificationOrderCreatedEventHandler(
            _serviceProvider.GetRequiredService<IEmailService>(),
            _serviceProvider.GetRequiredService<ILogger<NotificationOrderCreatedEventHandler>>());

        _eventBus.RegisterHandler(handler);
        await _eventBus.SubscribeAsync<NotificationOrderCreatedEvent, NotificationOrderCreatedEventHandler>();

        _logger.LogInformation("Notification consumer service started successfully.");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
