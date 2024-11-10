namespace eLib.Models.Results.Base;

public sealed record Error(int Code, string? Message = null)
{

}
