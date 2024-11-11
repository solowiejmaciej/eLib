using eLib.Models.Results.Base;

namespace eLib.Models.Results;

public class UserErrors
{
    public static readonly Error NotFound =  new(ErrorCodes.NotFound,
        $"User not found");
}