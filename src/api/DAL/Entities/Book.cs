using eLib.DomainEvents;

namespace eLib.DAL.Entities;

public sealed class Book : AggregateRoot
{
    private Book() : base(Guid.NewGuid()) { }

    public Book(
        string title,
        Guid authorId,
        BookDetails details) : base(Guid.NewGuid())
    {
        Title = title;
        AuthorId = authorId;
        Details = details;
        DetailsId = details.Id;
    }

    public string Title { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid DetailsId { get; private set; }
    public BookDetails Details { get; private set; }

    public static Book? Create(string title, Guid authorId, string description, string coverUrl)
    {
        var bookDetails = BookDetails.Create(description, coverUrl);
        var book = new Book(title, authorId, bookDetails);
        bookDetails.SetBookId(book.Id);

        book.RaiseDomainEvent(new BookCreatedEvent(book));
        return book;
    }
}