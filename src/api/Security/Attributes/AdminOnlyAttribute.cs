using Microsoft.AspNetCore.Authorization;

namespace eLib.Security.Attributes;

public class AdminOnlyAttribute : AuthorizeAttribute
{
    public AdminOnlyAttribute() : base("AdminOnly")
    {
    }
}