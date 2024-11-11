using Microsoft.AspNetCore.Authorization;

namespace eLib.Security.Requirements;

public class AdminOrCurrentUserRequirement : IAuthorizationRequirement
{

}