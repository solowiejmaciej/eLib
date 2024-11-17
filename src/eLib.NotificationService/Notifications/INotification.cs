using eLib.Common.Notifications;

namespace eLib.NotificationService.Notifications;

public interface INotification
{
    Guid Id { get; }
    Guid UserId { get; }
    string Message { get; }
    ENotificationType Type { get; }
    DateTime CreatedAt { get; }
    ENotificationChannel Channel { get; }
}