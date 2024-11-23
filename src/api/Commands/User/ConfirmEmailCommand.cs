using eLib.Auth.Providers;
using eLib.Common.Enums;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands.User;

public record ConfirmEmailCommand(string Code) : IResultCommand<Unit>;

public class ConfirmEmailCommandHandler : IResultCommandHandler<ConfirmEmailCommand, Unit>
{
    private readonly IUserInfoProvider _userInfoProvider;
    private readonly IUserRepository _userRepository;
    private readonly ITwoStepCodeRepository _twoStepCodeRepository;

    public ConfirmEmailCommandHandler(
        IUserInfoProvider userInfoProvider,
        IUserRepository userRepository,
        ITwoStepCodeRepository twoStepCodeRepository)
    {
        _userInfoProvider = userInfoProvider;
        _userRepository = userRepository;
        _twoStepCodeRepository = twoStepCodeRepository;
    }

    public async Task<Result<Unit, Error>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdWithDetailsAsync(userId, cancellationToken);
        if (user is null)
            return TwoStepCodeErrors.UserNotFound;

        var code = await _twoStepCodeRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (code is null)
            return TwoStepCodeErrors.CodeNotFound;

        var error = code.Use(ECodeType.ConfirmEmail, userId);
        if (error is not null)
            return error;

        user.Details.MarkEmailAsVerified();
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}