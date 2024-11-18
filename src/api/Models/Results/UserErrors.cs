using eLib.Models.Results.Base;

namespace eLib.Models.Results;

public class UserErrors
{
    public static readonly Error NotFound =  new(ErrorCodes.NotFound,
        $"User not found");
    public static readonly Error EmailNotUnique = new(ErrorCodes.InvalidData,
        $"Email is already taken");
    public static readonly Error PhoneNumberNotUnique = new(ErrorCodes.InvalidData,
        "Phone number is already taken");

}