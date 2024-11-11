using System.Text.Json.Serialization;

namespace eLib.Models.Dtos;

public class UserDetailsDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public bool IsAdmin { get; set; }
    public bool HasEmailNotifications { get; set; }
    public bool HasSmsNotifications { get; set; }
    public bool HasEmailVerified { get; set; }
    public bool HasSmsVerified { get; set; }
}