using System.Security.Claims;
using System.Text;
using eLib.DAL.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace eLib.Security;

public class AccessTokenCreator : IAccessTokenCreator
{
    private readonly IConfiguration _configuration;

    public AccessTokenCreator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateAsync(User user)
    {
        var claims = new Dictionary<string, object>
        {
            { "Id", user.Id },
            { "Name", user.Name },
            { "Surname", user.Surname },
            { "Email", user.Email },
            { "PhoneNumber", user.PhoneNumber },
            { "IsAdmin", user.Details.IsAdmin },
            { "HasPhoneNumberVerified", user.Details.HasPhoneNumberVerified },
            { "HasEmailVerified", user.Details.HasEmailVerified },
            { "HasSmsNotifications", user.Details.HasSmsNotifications },
            { "HasEmailNotifications", user.Details.HasEmailNotifications },
        };

        var secretKey = _configuration.GetValue<string>("AuthSettings:SecretKey");
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
    string CreateAsync(User user);
}