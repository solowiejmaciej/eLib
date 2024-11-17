using eLib.NotificationService.Notifications;

namespace eLib.NotificationService.Senders;

public interface INotificationSender
{
    Task SendAsync(INotification notification, CancellationToken cancellationToken);
}