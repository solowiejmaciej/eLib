using eLib.Auth.Security;
using eLib.Common.Dtos;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;

namespace eLib.Commands.Token;

public record CreateTokenFromPhoneNumberCommand() : IResultCommand<TokenDto>
{
    public string PhoneNumber { get; init; }
    public string Password { get; init; }
}

public class CreateTokenFromPhoneNumberCommandValidator : AbstractValidator<CreateTokenFromPhoneNumberCommand>
{
    public CreateTokenFromPhoneNumberCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\d{6,12}$")
            .WithMessage("Phone number must be between 6 and 12 digits.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(50);
    }
}

public class CreateTokenFromPhoneNumberCommandHandler : IResultCommandHandler<CreateTokenFromPhoneNumberCommand, TokenDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccessTokenCreator _accessTokenCreator;

    public CreateTokenFromPhoneNumberCommandHandler(
        IUserRepository userRepository,
        IAccessTokenCreator accessTokenCreator)
    {
        _userRepository = userRepository;
        _accessTokenCreator = accessTokenCreator;
    }

    public async Task<Result<TokenDto, Error>> Handle(CreateTokenFromPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPhoneNumberWithDetailsAsync(request.PhoneNumber, cancellationToken);
        if (user == null)
        {
            return TokenErrors.InvalidEmailOrPassword;
        }

        if (!user.Details.VerifyPassword(request.Password))
        {
            return TokenErrors.InvalidEmailOrPassword;
        }

        if (!user.Details.HasPhoneNumberVerified)
        {
            return TokenErrors.PhoneNumberNotVerified;
        }

        var userInfo = user.MapToDto().MapToUserInfo();

        return new TokenDto(_accessTokenCreator.CreateAsync(userInfo));
    }
}