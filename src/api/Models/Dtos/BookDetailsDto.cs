using System.Text.Json.Serialization;

namespace eLib.Models.Dtos;

public class BookDetailsDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string CoverUrl { get; set; }
    public int Quantity { get; set; }
}