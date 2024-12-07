using eLib.Common.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;

namespace eLib.DAL.Entities;

public class Review : Entity
{
    private const int MAX_CONTENT_LENGTH = 300;
    private Review() : base(Guid.NewGuid()) { }

    private Review(
        string content,
        int rating,
        Guid bookId,
        Guid userId) : base(Guid.NewGuid())
    {
        Content = content;
        Rating = rating;
        BookId = bookId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public string Content { get; private set; }
    public int Rating { get; private set; }
    public Guid BookId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Book Book { get; private set; }
    public User User { get; private set; }

    public static Result<Review, Error> Create(
        string content,
        int rating,
        Guid bookId,
        Guid userId)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return ReviewErrors.ContentEmpty;
        }

        if (content.Length > MAX_CONTENT_LENGTH)
        {
            return ReviewErrors.ContentTooLong;
        }

        if (rating < 1 || rating > 5)
        {
            return ReviewErrors.InvalidRating;
        }

        var review = new Review(content, rating, bookId, userId);
        return review;
    }

    public Result<bool, Error> Update(
        string requestContent,
        int requestRating)
    {
        if (string.IsNullOrWhiteSpace(requestContent))
        {
            return ReviewErrors.ContentEmpty;
        }

        if (requestContent.Length > MAX_CONTENT_LENGTH)
        {
            return ReviewErrors.ContentTooLong;
        }

        if (requestRating < 1 || requestRating > 5)
        {
            return ReviewErrors.InvalidRating;
        }


        Content = requestContent;
        Rating = requestRating;
        UpdatedAt = DateTime.UtcNow;

        return true;
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
    }

    public ReviewDto MapToDto()
        => new()
        {
            Id = Id,
            Content = Content,
            Rating = Rating,
            BookId = BookId,
            UserId = UserId,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            DeletedAt = DeletedAt
        };

}