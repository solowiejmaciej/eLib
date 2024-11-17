using eLib.NotificationService.Notifications;

namespace eLib.NotificationService.Senders.System;

public class SystemNotificationSender : ISystemNotificationSender
{
    private readonly ILogger<SystemNotificationSender> _logger;

    public SystemNotificationSender(
        ILogger<SystemNotificationSender> logger
        )
    {
        _logger = logger;
    }

    public async Task SendAsync(INotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Sending system notification: {notification.Id}");
    }
}

public interface ISystemNotificationSender : INotificationSender;