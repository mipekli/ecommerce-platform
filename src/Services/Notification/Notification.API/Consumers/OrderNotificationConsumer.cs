using MassTransit;
using Notification.API.Services;
using BuildingBlocks.Shared.Messaging;

namespace Notification.API.Consumers;

public record NotificationOrderCreatedEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}

public class OrderNotificationConsumer : IConsumer<NotificationOrderCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderNotificationConsumer> _logger;

    public OrderNotificationConsumer(
        IEmailService emailService,
        ILogger<OrderNotificationConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NotificationOrderCreatedEvent> context)
    {
        _logger.LogInformation("Processing notification for order {OrderNumber}", context.Message.OrderNumber);
        await _emailService.SendOrderConfirmationAsync(
            "customer@example.com",
            context.Message.OrderNumber,
            context.Message.TotalAmount,
            context.CancellationToken);
    }
}
