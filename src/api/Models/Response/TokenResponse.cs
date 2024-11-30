using eLib.Common.Dtos;

namespace eLib.Models.Response;

public class TokenResponse
{
    public TokenDto AccessToken { get; set; }
    public UserDto User { get; set; }
}