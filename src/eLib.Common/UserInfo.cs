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
    public ENotificationChannel NotificationChannel { get; set; }

    public static UserInfo Create(
        Guid userId,
        ENotificationChannel notificationChannel,
        string? phoneNumber,
        string? email)
    {
        return new UserInfo(
            userId,
            string.Empty,
            string.Empty,
            email ?? string.Empty,
            phoneNumber ?? string.Empty,
            false,
            true,
            true,
            notificationChannel
        );
    }
}