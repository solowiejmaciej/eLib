using eLib.Auth.Providers;
using eLib.Common;
using eLib.Common.Enums;
using eLib.Common.Notifications;
using eLib.DAL.Entities;
using eLib.DAL.Repositories;
using eLib.Events.Events.Notifications;
using eLib.Events.Services;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands.User;

public record SendConfirmEmailCommand : IResultCommand<Unit>;

public class SendConfirmEmailCommandHandler : IResultCommandHandler<SendConfirmEmailCommand, Unit>
{
    private readonly IUserInfoProvider _userInfoProvider;
    private readonly IUserRepository _userRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ITwoStepCodeRepository _twoStepCodeRepository;

    public SendConfirmEmailCommandHandler(
        IUserInfoProvider userInfoProvider,
        IUserRepository userRepository,
        IEventPublisher eventPublisher,
        ITwoStepCodeRepository twoStepCodeRepository)
    {
        _userInfoProvider = userInfoProvider;
        _userRepository = userRepository;
        _eventPublisher = eventPublisher;
        _twoStepCodeRepository = twoStepCodeRepository;
    }

    public async Task<Result<Unit, Error>> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdWithDetailsAsync(currentUserId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        if(user.Details.HasEmailVerified)
            return UserErrors.EmailAlreadyVerified;

        var userInfo = user.MapToDto().MapToUserInfo();

        var code = TwoStepCode.Create(currentUserId, ECodeType.ConfirmEmail);
        await _twoStepCodeRepository.AddAsync(code, cancellationToken);
        await _twoStepCodeRepository.SaveChangesAsync(cancellationToken);

        var associatedObjects = new List<SerializedObject>
        {
            new(code.MapToDto()),
            new(userInfo)
        };

        await _eventPublisher.PublishAsync(new SendEmailNotificationEvent(ENotificationType.ConfirmEmail, userInfo, associatedObjects), cancellationToken);

        return Unit.Value;
    }
}