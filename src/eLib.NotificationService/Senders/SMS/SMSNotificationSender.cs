using eLib.NotificationService.Notifications;

namespace eLib.NotificationService.Senders.SMS;

public class SMSNotificationSender : ISMSNotificationSender
{
    private readonly ILogger<SMSNotificationSender> _logger;

    public SMSNotificationSender(ILogger<SMSNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(INotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending sms notification: {notification.Id}");
    }
}

public interface ISMSNotificationSender : INotificationSender;