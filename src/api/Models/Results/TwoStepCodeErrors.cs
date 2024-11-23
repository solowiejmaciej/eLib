using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Models.Results;

public class TwoStepCodeErrors
{
    public static readonly Error UserNotFound =  new(ErrorCodes.InvalidData,
        $"User not found");

    public static readonly Error CodeNotFound =  new(ErrorCodes.InvalidData,
        $"Code not found");
}