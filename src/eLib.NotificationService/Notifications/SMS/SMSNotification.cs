using eLib.Common.Notifications;

namespace eLib.NotificationService.Notifications.SMS;

internal class SMSNotification : INotification
{
    private SMSNotification(
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
        Channel = ENotificationChannel.SMS;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public ENotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ENotificationChannel Channel { get; private set; }

    public static SMSNotification Create(
        Guid userId,
        string message,
        ENotificationType type
    )
    {
        return new SMSNotification(userId, message, type);
    }
}