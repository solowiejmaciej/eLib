using eLib.NotificationService.DAL;

namespace eLib.NotificationService.Senders;

public interface INotificationSender
{
    Task SendAsync(Notification notification, CancellationToken cancellationToken);
}