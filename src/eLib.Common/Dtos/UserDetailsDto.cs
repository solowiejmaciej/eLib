using System.Text.Json.Serialization;
using eLib.Common.Notifications;

namespace eLib.Common.Dtos;

public class UserDetailsDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public bool IsAdmin { get; set; }
    public bool HasEmailVerified { get; set; }
    public bool HasPhoneNumberVerified { get; set; }
    public ENotificationChannel NotificationChannel { get; set; }
}