using eLib.NotificationService.Notifications;

namespace eLib.NotificationService.Senders.Email;

public class EmailNotificationSender : IEmailNotificationSender
{
    private readonly ILogger<EmailNotificationSender> _logger;

    public EmailNotificationSender(ILogger<EmailNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(INotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending sms notification: {notification.Id}");
    }
}

public interface IEmailNotificationSender : INotificationSender;