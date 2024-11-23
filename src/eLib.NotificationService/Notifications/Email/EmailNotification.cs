using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.DAL;

namespace eLib.NotificationService.Notifications.Email;

internal abstract class EmailNotification : INotification
{
    public Guid Id { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public ENotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ENotificationChannel Channel { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? DeletedAt { get; set; }
    public string Title { get; private set; }

    public static Notification Create(
        string message,
        ENotificationType type,
        UserInfo userInfo
        )
    {
        var notification = Notification.Create(
            userInfo,
            message,
            type,
            ENotificationChannel.Email);

        return notification;
    }
}