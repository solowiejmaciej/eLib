namespace eLib.DAL.Entities;

public sealed class BookDetails : Entity
{
    private BookDetails(string description, string coverUrl) : base(Guid.NewGuid())
    {
        Description = description;
        CoverUrl = coverUrl;
    }

    public string Description { get; private set; }
    public string CoverUrl { get; private set; }
    public Guid BookId { get; private set; }

    public static BookDetails Create(string description, string coverUrl)
    {
        var bookDetails = new BookDetails(description, coverUrl);
        return bookDetails;
    }

    public void SetBookId(Guid bookId)
    {
        BookId = bookId;
    }
}