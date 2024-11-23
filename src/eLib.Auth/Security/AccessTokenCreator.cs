using System.Text;
using eLib.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace eLib.Auth.Security;

public class AccessTokenCreator : IAccessTokenCreator
{
    private readonly IConfiguration _configuration;

    public AccessTokenCreator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateAsync(UserInfo user)
    {
        var claims = new Dictionary<string, object>
        {
            { "Id", user.Id },
            { "Name", user.Name },
            { "Surname", user.Surname },
            { "Email", user.Email },
            { "PhoneNumber", user.PhoneNumber },
            { "IsAdmin", user.IsAdmin },
            { "HasPhoneNumberVerified", user.HasPhoneNumberVerified },
            { "HasEmailVerified", user.HasEmailVerified },
            { "NotificationChannel", (int)user.NotificationChannel }
        };

        var secretKey = _configuration.GetSection("AuthSettings:SecretKey").Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "MyIssuer",
            Audience = "MyAudience",
            Claims = claims,
            IssuedAt = null,
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(120),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JsonWebTokenHandler();
        handler.SetDefaultTimesOnTokenCreation = false;
        return handler.CreateToken(descriptor);
    }
}

public interface IAccessTokenCreator
{
    string CreateAsync(UserInfo user);
}