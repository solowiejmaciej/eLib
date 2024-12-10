namespace eLib.Common.Dtos;

public class ReadingListEntryBookDto
{
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }
    public int Progress { get; set; }
    public bool IsFinished { get; set; }
    public DateTime DateAdded { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string CoverUrl { get; set; }
    public Guid AuthorId { get; set; }
}