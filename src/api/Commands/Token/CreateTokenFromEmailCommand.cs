using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using eLib.Security;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Token;

public record CreateTokenFromEmailCommand() : IRequest<Result<TokenDto, Error>>
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

public class CreateTokenFromEmailCommandHandler : IRequestHandler<CreateTokenFromEmailCommand, Result<TokenDto, Error>>
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

        return new TokenDto(_accessTokenCreator.CreateAsync(user));
    }
}
