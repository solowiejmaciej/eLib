using System.Linq;
using System.Threading.Tasks;
using eLib.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace eLib.Security.Handlers;

public class AdminOrCurrentUserHandler : AuthorizationHandler<AdminOrCurrentUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrCurrentUserRequirement requirement)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "Id");
        var isAdminClaim = context.User.Claims.FirstOrDefault(c => c.Type == "IsAdmin");

        if (userIdClaim != null && context.Resource is HttpContext httpContext)
        {
            var routeData = httpContext.Request.RouteValues;
            if (routeData.TryGetValue("id", out var routeId) && routeId.ToString() == userIdClaim.Value)
            {
                context.Succeed(requirement);
            }
        }

        if (isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out var isAdmin) && isAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}