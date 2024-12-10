using eLib.Models.Results.Base;

namespace eLib.Models.Results;

public class ReadingListErrors
{
    public static readonly Error NotFound = new(ErrorCodes.NotFound,
        $"Book or user not found in reading list");

    public static readonly Error AlreadyExists = new(ErrorCodes.InvalidOperation,
        "Book already exists in reading list");

    public static readonly Error InvalidProgress = new(ErrorCodes.InvalidOperation,
        "Progress must be between 0 and 100");

    public static readonly Error AlreadyFinished = new(ErrorCodes.InvalidOperation,
        "Reading list entry is already finished");

    public static readonly Error AlreadyUnfinished = new(ErrorCodes.InvalidOperation,
        "Reading list entry is already unfinished");
}