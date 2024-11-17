using Microsoft.AspNetCore.Authorization;

namespace eLib.Auth.Security.Attributes;

public class AdminOrCurrentUserAttribute : AuthorizeAttribute
{
    public AdminOrCurrentUserAttribute() : base("AdminOrCurrentUser")
    {

    }
}