namespace eLib.Models.Dtos;

public class AuthorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime Birthday { get; set; }
    public AuthorDetailsDto Details { get; set; }
}