using Microsoft.Extensions.Logging;

namespace Notification.Infrastructure.Services;

public interface IEmailService
{
    Task SendOrderConfirmationAsync(string to, string orderNumber, decimal totalAmount, CancellationToken cancellationToken = default);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(string to, string orderNumber, decimal totalAmount, CancellationToken cancellationToken = default)
    {
        // In production, integrate with SMTP or email provider (SendGrid, etc.)
        _logger.LogInformation(
            "Sending order confirmation email to {To}: Order {OrderNumber} with total {TotalAmount:C}",
            to, orderNumber, totalAmount);

        await Task.Delay(100, cancellationToken); // Simulate email sending
    }
}
