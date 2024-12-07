using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Models.Results;

public class ReviewErrors
{
    public static readonly Error ContentEmpty = new(ErrorCodes.InvalidData,
        $"Content cannot be empty.");

    public static readonly Error ContentTooLong = new(ErrorCodes.InvalidData,
        "Content is too long.");

    public static readonly Error InvalidRating = new(ErrorCodes.InvalidData,
        "Rating must be between 1 and 5.");

    public static readonly Error NotFound = new(ErrorCodes.InvalidData,
        "Review not found.");
}