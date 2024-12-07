using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Models.Results;

public class UserErrors
{
    public static readonly Error NotFound =  new(ErrorCodes.NotFound,
        $"User not found");
    public static readonly Error EmailNotUnique = new(ErrorCodes.InvalidData,
        $"Email is already taken");
    public static readonly Error PhoneNumberNotUnique = new(ErrorCodes.InvalidData,
        "Phone number is already taken");

    public static readonly Error EmailAlreadyVerified = new(ErrorCodes.InvalidOperation,
        "Email is already verified");
    public static readonly Error PhoneNumberAlreadyVerified = new(ErrorCodes.InvalidOperation,
        "Phone number is already verified");

    public static readonly Error InvalidPassword = new(ErrorCodes.InvalidData,
        "Invalid password");

    public static readonly Error NotAuthorized = new(ErrorCodes.InvalidOperation,
        "User is not authorized to perform this operation");
}