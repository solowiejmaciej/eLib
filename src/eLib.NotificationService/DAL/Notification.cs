using eLib.Common;
using eLib.Common.Notifications;
using eLib.NotificationService.Notifications;

namespace eLib.NotificationService.DAL;

public class Notification : INotification
{
    public Notification() { }
    private Notification(
        UserInfo userInfo,
        string message,
        ENotificationType type,
        ENotificationChannel channel
    )
    {
        Id = Guid.NewGuid();
        UserId = userInfo.Id;
        Message = message;
        Type = type;
        Title = type.ToString();
        CreatedAt = DateTime.UtcNow;
        Channel = channel;
        Details = NotificationDetails.Create(userInfo.Id, userInfo.Email, userInfo.PhoneNumber);
        Details.SetNotificationId(Id);
    }

    public static Notification Create(
        UserInfo userInfo,
        string message,
        ENotificationType type,
        ENotificationChannel channel
    )
    {
        return new Notification(userInfo, message, type, channel);
    }

    public Guid Id { get; private set; }
    public string? Title { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public ENotificationType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ENotificationChannel Channel { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? FailedAt { get; private set; }
    public NotificationDetails Details { get; private set; }

    public void MarkAsRead()
    {
        ReadAt = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        SentAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        FailedAt = null;
    }

    public void Fail()
    {
        FailedAt = DateTime.UtcNow;
    }
}