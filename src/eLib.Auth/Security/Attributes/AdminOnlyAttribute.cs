using Microsoft.AspNetCore.Authorization;

namespace eLib.Auth.Security.Attributes;

public class AdminOnlyAttribute : AuthorizeAttribute
{
    public AdminOnlyAttribute() : base("AdminOnly")
    {
    }
}