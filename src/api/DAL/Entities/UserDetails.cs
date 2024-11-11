using eLib.Models.Dtos;

namespace eLib.DAL.Entities;

public class UserDetails : Entity
{
    private UserDetails() : base(Guid.NewGuid()) { }

    private UserDetails(
        string password,
        bool isAdmin,
        bool hasEmailNotifications,
        bool hasSmsNotifications,
        bool hasEmailVerified,
        bool hasPhoneNumberVerified) : base(Guid.NewGuid())
    {
        Password = password;
        IsAdmin = isAdmin;
        HasEmailNotifications = hasEmailNotifications;
        HasSmsNotifications = hasSmsNotifications;
        HasEmailVerified = hasEmailVerified;
        HasPhoneNumberVerified = hasPhoneNumberVerified;
    }

    public Guid UserId { get; private set; }
    private string Password { get; set; }
    public bool IsAdmin { get; set; }
    public bool HasEmailNotifications { get; set; }
    public bool HasSmsNotifications { get; set; }
    public bool HasPhoneNumberVerified { get; set; }
    public bool HasEmailVerified { get; set; }

    public static UserDetails Create(
        string password,
        bool isAdmin,
        bool hasEmailNotifications,
        bool hasSmsNotifications,
        bool hasEmailVerified,
        bool hasPhoneNumberVerfied)
    {
        var userDetails = new UserDetails(password, isAdmin, hasEmailNotifications, hasSmsNotifications, hasEmailVerified, hasPhoneNumberVerfied);
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
            HasEmailNotifications = HasEmailNotifications,
            HasSmsNotifications = HasSmsNotifications,
            HasEmailVerified = HasEmailVerified,
            HasSmsVerified = HasPhoneNumberVerified
        };
    }
}