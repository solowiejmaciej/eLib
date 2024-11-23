using eLib.Auth.Providers;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.User;

public record ChangePasswordCommand(string OldPassword, string NewPassword, string ConfirmNewPassword) : IResultCommand<Unit>;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty();
        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword);
    }
}

public class ChangePasswordCommandHandler : IResultCommandHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IUserInfoProvider userInfoProvider)
    {
        _userRepository = userRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task<Result<Unit, Error>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdWithDetailsAsync(userId, cancellationToken);
        if (user == null)
            return UserErrors.NotFound;

        if (!user.Details.VerifyPassword(request.OldPassword))
            return UserErrors.InvalidPassword;

        user.Details.ChangePassword(request.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}