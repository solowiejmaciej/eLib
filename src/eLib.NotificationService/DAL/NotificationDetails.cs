namespace eLib.NotificationService.DAL;

public class NotificationDetails
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public Guid NotificationId { get; private set; }

    public NotificationDetails() { }

    private NotificationDetails(
        Guid userId,
        string email,
        string phoneNumber)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static NotificationDetails Create(
        Guid userId,
        string email,
        string phoneNumber)
    {
        return new NotificationDetails(userId, email, phoneNumber);
    }

    public void SetNotificationId(Guid notificationId)
    {
        NotificationId = notificationId;
    }
}