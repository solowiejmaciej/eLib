namespace eLib.Common.Dtos;

public class ReviewDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public int Rating { get; set; }
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}