using System.Text.Json.Serialization;
using eLib.Common.Notifications;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.User;

public record ChangeNotificationChannelCommand(Guid UserId,ENotificationChannel NotificationChannel) : IResultCommand<Unit>
{
    [JsonIgnore]
    public Guid UserId { get; set; } = UserId;
    public ENotificationChannel NotificationChannel { get; set; } = NotificationChannel;
}

public class ChangeNotificationChannelCommandValidator : AbstractValidator<ChangeNotificationChannelCommand>
{
    public ChangeNotificationChannelCommandValidator()
    {
        RuleFor(x => x.NotificationChannel).IsInEnum();
    }
}

public class ChangeNotificationChannelCommandHandler : IResultCommandHandler<ChangeNotificationChannelCommand, Unit>
{
    private readonly IUserRepository _userRepository;

    public ChangeNotificationChannelCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Unit, Error>> Handle(ChangeNotificationChannelCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId, cancellationToken);
        if (user is null)
            return UserErrors.NotFound;

        user.Details.ChangeNotificationChannel(request.NotificationChannel);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}