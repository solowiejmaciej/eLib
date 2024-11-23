namespace eLib.Common.Dtos;

public class TokenDto(string token)
{
    public string Token { get; private set; } = token;
}