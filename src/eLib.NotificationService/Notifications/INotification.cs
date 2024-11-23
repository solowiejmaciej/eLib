using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.DAL;

namespace eLib.NotificationService.Notifications;

public interface INotification
{
    Guid Id { get; }
    string? Title { get; }
    string Message { get; }
    DateTime CreatedAt { get; }
    DateTime? ReadAt { get; }
    ENotificationType Type { get; }
    ENotificationChannel Channel { get; }
    DateTime? SentAt { get; }
    DateTime? DeletedAt { get; }
}