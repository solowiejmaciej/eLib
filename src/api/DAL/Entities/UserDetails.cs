using eLib.Common.Dtos;
using eLib.Common.Notifications;

namespace eLib.DAL.Entities;

public class UserDetails : Entity
{
    private UserDetails() : base(Guid.NewGuid()) { }

    private UserDetails(
        string password,
        bool isAdmin,
        bool hasEmailVerified,
        bool hasPhoneNumberVerified,
        ENotificationChannel notificationChannel) : base(Guid.NewGuid())
    {
        Password = password;
        IsAdmin = isAdmin;
        HasEmailVerified = hasEmailVerified;
        HasPhoneNumberVerified = hasPhoneNumberVerified;
        NotificationChannel = notificationChannel;
    }

    public Guid UserId { get; private set; }
    public string Password { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool HasPhoneNumberVerified { get; private set; }
    public bool HasEmailVerified { get; private set; }
    public ENotificationChannel NotificationChannel { get; private set; }

    public static UserDetails Create(
        string password,
        bool isAdmin,
        bool hasEmailVerified,
        bool hasPhoneNumberVerified,
        ENotificationChannel notificationChannel)
    {
        var userDetails = new UserDetails(password, isAdmin, hasEmailVerified, hasPhoneNumberVerified, notificationChannel);
        userDetails.EncryptPassword();
        return userDetails;
    }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    private void EncryptPassword()
    {
        Password = BCrypt.Net.BCrypt.HashPassword(Password);
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, Password);
    }

    public UserDetailsDto MapToDto()
    {
        return new UserDetailsDto
        {
            Id = Id,
            IsAdmin = IsAdmin,
            HasEmailVerified = HasEmailVerified,
            HasPhoneNumberVerified = HasPhoneNumberVerified,
            NotificationChannel = NotificationChannel
        };
    }

    public void MarkEmailAsUnverified()
    {
        HasEmailVerified = false;
    }

    public void MarkPhoneNumberAsUnverified()
    {
        HasPhoneNumberVerified = false;
    }
}