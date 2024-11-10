using eLib.Models.Results.Base;

namespace eLib.Models.Results;

public class AuthorErrors
{
    public static readonly Error NotFound =  new(ErrorCodes.NotFound,
        "Author not found");
}