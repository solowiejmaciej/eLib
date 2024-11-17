using System.Linq;
using System.Threading.Tasks;
using eLib.Auth.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace eLib.Security.Handlers;

public class AdminOnlyHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        var isAdminClaim = context.User.Claims.FirstOrDefault(c => c.Type == "IsAdmin");

        if (isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out var isAdmin) && isAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}