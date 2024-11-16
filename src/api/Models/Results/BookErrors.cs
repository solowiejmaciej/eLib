using eLib.Models.Results.Base;

namespace eLib.Models.Results;

public class BookErrors
{
    public static readonly Error NotFound =  new(ErrorCodes.NotFound,
        $"Book not found");

    public static readonly Error NoAvailableCopies = new(ErrorCodes.InvalidOperation,
        $"No available copies");
}