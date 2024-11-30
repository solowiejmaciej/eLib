using eLib.Auth.Security;
using eLib.Common.Dtos;
using eLib.DAL.Repositories;
using eLib.Models.Response;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Commands.Token;

public record CreateTokenFromEmailCommand() : IResultCommand<TokenResponse>
{
    public string Email { get; init; }
    public string Password { get; init; }
}

public class CreateTokenFromEmailCommandValidator : AbstractValidator<CreateTokenFromEmailCommand>
{
    public CreateTokenFromEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(50);
    }
}

public class CreateTokenFromEmailCommandHandler : IResultCommandHandler<CreateTokenFromEmailCommand, TokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccessTokenCreator _accessTokenCreator;

    public CreateTokenFromEmailCommandHandler(
        IUserRepository userRepository,
        IAccessTokenCreator accessTokenCreator)
    {
        _userRepository = userRepository;
        _accessTokenCreator = accessTokenCreator;
    }

    public async Task<Result<TokenResponse, Error>> Handle(CreateTokenFromEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailWithDetailsAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return TokenErrors.InvalidEmailOrPassword;
        }

        if (!user.Details.VerifyPassword(request.Password))
        {
            return TokenErrors.InvalidEmailOrPassword;
        }

        var userDto = user.MapToDto();
        var userInfo = user.MapToDto().MapToUserInfo();
        var token = new TokenDto(_accessTokenCreator.CreateAsync(userInfo));

        var result = new TokenResponse()
        {
            AccessToken = token,
            User = userDto
        };

        return result;
    }
}
