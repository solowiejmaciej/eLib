using eLib.Common.Notifications;

namespace eLib.Common;

public class UserInfo
{
    public UserInfo(
        Guid id,
        string name,
        string surname,
        string email,
        string phoneNumber,
        bool isAdmin,
        bool hasPhoneNumberVerified,
        bool hasEmailVerified,
        bool hasSmsNotifications,
        bool hasEmailNotifications,
        ENotificationChannel notificationChannel)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Email = email;
        PhoneNumber = phoneNumber;
        IsAdmin = isAdmin;
        HasPhoneNumberVerified = hasPhoneNumberVerified;
        HasEmailVerified = hasEmailVerified;
        HasSmsNotifications = hasSmsNotifications;
        HasEmailNotifications = hasEmailNotifications;
        NotificationChannel = notificationChannel;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool HasPhoneNumberVerified { get; private set; }
    public bool HasEmailVerified { get; private set; }
    public bool HasSmsNotifications { get; private set; }
    public bool HasEmailNotifications { get; private set; }
    public ENotificationChannel NotificationChannel { get; set; }
}