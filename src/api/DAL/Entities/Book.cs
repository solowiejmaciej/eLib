using eLib.Commands;
using eLib.Commands.Book;
using eLib.DomainEvents;
using eLib.Models.Dtos;

namespace eLib.DAL.Entities;

public sealed class Book : AggregateRoot
{
    private Book() : base(Guid.NewGuid()) { }

    private Book(
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

    public static Book Create(string title, Guid authorId, string description, string coverUrl, int quantity)
    {
        var bookDetails = BookDetails.Create(description, coverUrl, quantity);
        var book = new Book(title, authorId, bookDetails);
        bookDetails.SetBookId(book.Id);

        book.RaiseDomainEvent(new BookCreatedEvent(book));
        return book;
    }

    public static Book Create(CreateBookCommand createBookCommand)
         => Create(createBookCommand.Title, createBookCommand.AuthorId, createBookCommand.Description, createBookCommand.CoverImageUrl, createBookCommand.Quantity);

    public void Update(UpdateBookCommand request)
    {
        Title = request.Title;
        AuthorId = request.AuthorId;
        Details.Update(request.Description, request.CoverImageUrl, request.Quantity);
    }

    public BookDto MapToDto()
        => new()
        {
            Id = Id,
            Title = Title,
            AuthorId = AuthorId,
            Details = Details.MapToDto()
        };
}