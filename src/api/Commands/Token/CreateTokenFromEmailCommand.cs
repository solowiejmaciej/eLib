using eLib.Auth.Security;
using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Commands.Token;

public record CreateTokenFromEmailCommand() : IResultCommand<TokenDto>
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

public class CreateTokenFromEmailCommandHandler : IResultCommandHandler<CreateTokenFromEmailCommand, TokenDto>
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

    public async Task<Result<TokenDto, Error>> Handle(CreateTokenFromEmailCommand request, CancellationToken cancellationToken)
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

        if (!user.Details.HasEmailVerified)
        {
            return TokenErrors.EmailNotVerified;
        }

        var userInfo = user.MapToDto().MapToUserInfo();

        return new TokenDto(_accessTokenCreator.CreateAsync(userInfo));
    }
}
