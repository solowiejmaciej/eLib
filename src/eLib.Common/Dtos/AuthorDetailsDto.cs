using System.Text.Json.Serialization;

namespace eLib.Common.Dtos;

public class AuthorDetailsDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Biography { get; set; }
    public string PhotoUrl { get; set; }
}