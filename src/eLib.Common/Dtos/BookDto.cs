namespace eLib.Common.Dtos;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid AuthorId { get; set; }
    public BookDetailsDto Details { get; set; }
}