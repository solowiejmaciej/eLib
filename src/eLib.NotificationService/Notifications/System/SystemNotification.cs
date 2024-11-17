using eLib.Common.Notifications;

namespace eLib.NotificationService.Notifications.System;

internal class SystemNotification : INotification
{
    private SystemNotification(
        Guid userId,
        string message,
        ENotificationType type
        )
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Message = message;
        Type = type;
        CreatedAt = DateTime.UtcNow;
        Channel = ENotificationChannel.System;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public ENotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ENotificationChannel Channel { get; private set; }

    public static SystemNotification Create(
        Guid userId,
        string message,
        ENotificationType type
        )
    {
        return new SystemNotification(userId, message, type);
    }
}