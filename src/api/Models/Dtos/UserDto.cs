using eLib.Common;

namespace eLib.Models.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public UserDetailsDto Details { get; set; }

    public UserInfo MapToUserInfo()
    {
        return new UserInfo(
            Id,
            Name,
            Surname,
            Email,
            PhoneNumber,
            Details.IsAdmin,
            Details.HasPhoneNumberVerified,
            Details.HasEmailVerified,
            Details.HasSmsNotifications,
            Details.HasEmailNotifications);
    }
}