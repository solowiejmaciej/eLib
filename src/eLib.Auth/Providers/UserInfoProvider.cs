using System.Security.Claims;
using eLib.Common;
using eLib.Common.Notifications;
using Microsoft.AspNetCore.Http;

namespace eLib.Auth.Providers;

public class UserInfoProvider : IUserInfoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserInfo GetCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || httpContext.User == null || !httpContext.User.Identity.IsAuthenticated)
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        var user = httpContext.User;

        return new UserInfo(
            id: Guid.Parse(GetClaim(user, "Id")),
            name: GetClaim(user, "Name"),
            surname: GetClaim(user, "Surname"),
            email: GetClaim(user, "Email"),
            phoneNumber: GetClaim(user, "PhoneNumber"),
            isAdmin: bool.Parse(GetClaim(user, "IsAdmin")),
            hasPhoneNumberVerified: bool.Parse(GetClaim(user, "HasPhoneNumberVerified")),
            hasEmailVerified: bool.Parse(GetClaim(user, "HasEmailVerified")),
            hasSmsNotifications: bool.Parse(GetClaim(user, "HasSmsNotifications")),
            hasEmailNotifications: bool.Parse(GetClaim(user, "HasEmailNotifications")),
            notificationChannel: Enum.Parse<ENotificationChannel>(GetClaim(user, "NotificationChannel"))
        );
    }

    private static string GetClaim(ClaimsPrincipal user, string claimType)
    {
        return user.FindFirst(claimType)?.Value
               ?? throw new InvalidOperationException($"Claim '{claimType}' not found.");
    }
}

public interface IUserInfoProvider
{
    UserInfo GetCurrentUser();
}