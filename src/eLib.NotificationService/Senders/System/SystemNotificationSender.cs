
using eLib.NotificationService.DAL;

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

    public async Task SendAsync(Notification notification, CancellationToken cancellationToken)
    {
        notification.MarkAsSent();
    }
}

public interface ISystemNotificationSender : INotificationSender;