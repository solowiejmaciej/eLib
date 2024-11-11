using Microsoft.AspNetCore.Authorization;

namespace eLib.Security.Attributes;

public class AdminOrCurrentUserAttribute : AuthorizeAttribute
{
    public AdminOrCurrentUserAttribute() : base("AdminOrCurrentUser")
    {

    }
}