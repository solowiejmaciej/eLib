namespace eLib.Models.Dtos;

public class BookDetailsDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string CoverUrl { get; set; }
    public int Quantity { get; set; }
}