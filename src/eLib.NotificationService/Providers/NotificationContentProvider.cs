using eLib.Common.Notifications;

namespace eLib.NotificationService.Providers;

public class NotificationContentProvider : INotificationContentProvider
{
    public string GetContent(ENotificationType notificationType)
    {
        return "Notification content";
    }
}

public interface INotificationContentProvider
{
    string GetContent(ENotificationType notificationType);
}