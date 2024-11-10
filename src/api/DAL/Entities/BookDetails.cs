using eLib.Models.Dtos;

namespace eLib.DAL.Entities;

public sealed class BookDetails : Entity
{
    private BookDetails(
        string description,
        string coverUrl,
        int quantity) : base(Guid.NewGuid())
    {
        Description = description;
        CoverUrl = coverUrl;
        Quantity = quantity;
    }

    public string Description { get; private set; }
    public string CoverUrl { get; private set; }
    public int Quantity { get; private set; }
    public Guid BookId { get; private set; }

    public static BookDetails Create(string description, string coverUrl, int quantity)
    {
        var bookDetails = new BookDetails(description, coverUrl, quantity);
        return bookDetails;
    }

    public void SetBookId(Guid bookId)
    {
        BookId = bookId;
    }

    public BookDetailsDto MapToDto()
    {
        return new BookDetailsDto
        {
            Id = Id,
            Description = Description,
            CoverUrl = CoverUrl,
            Quantity = Quantity
        };
    }

    public void Update(string requestDescription, string requestCoverImageUrl, int requestQuantity)
    {
        Quantity = requestQuantity;
        Description = requestDescription;
        CoverUrl = requestCoverImageUrl;
    }
}