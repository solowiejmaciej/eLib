using eLib.Common.Notifications;

namespace eLib.NotificationService.Notifications.Email;

internal class EmailNotification : INotification
{
    private EmailNotification(
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
        Channel = ENotificationChannel.Email;
        Title = type.ToString();
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public ENotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ENotificationChannel Channel { get; private set; }
    public string Title { get; private set; }

    public static EmailNotification Create(
        Guid userId,
        string message,
        ENotificationType type
    )
    {
        return new EmailNotification(userId, message, type);
    }
}