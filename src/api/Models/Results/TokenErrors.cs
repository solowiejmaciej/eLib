using eLib.Models.Results.Base;

namespace eLib.Models.Results;

public class TokenErrors
{
    public static readonly Error InvalidEmailOrPassword =  new(ErrorCodes.NotFound,
        $"Invalid email or password");

    public static readonly Error InvalidPhoneNumberOrPassword =  new(ErrorCodes.NotFound,
        $"Invalid phone number or password");

    public static readonly Error EmailNotVerified =  new(ErrorCodes.InvalidOperation,
        $"Email not verified");

    public static readonly Error PhoneNumberNotVerified =  new(ErrorCodes.InvalidOperation,
        "Phone number not verified");

}