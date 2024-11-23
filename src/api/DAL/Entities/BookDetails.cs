using eLib.Common.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;

namespace eLib.DAL.Entities;

public sealed class BookDetails : Entity
{
    private BookDetails() : base(Guid.NewGuid()) { }

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

    public void Update(string requestDescription, string requestCoverImageUrl, int requestQuantity)
    {
        Quantity = requestQuantity;
        Description = requestDescription;
        CoverUrl = requestCoverImageUrl;
    }

    public BookDetailsDto MapToDto()
        => new()
        {
            Id = Id,
            Description = Description,
            CoverUrl = CoverUrl,
            Quantity = Quantity
        };

    public Error? DecreaseAvailableCopies()
    {
        if (Quantity == 0)
            return BookErrors.NoAvailableCopies;

        Quantity--;
        return null;
    }

    public void IncreaseAvailableCopies()
    {
        Quantity++;
    }
}