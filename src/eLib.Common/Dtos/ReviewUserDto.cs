namespace eLib.Common.Dtos;

public class ReviewUserDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
}