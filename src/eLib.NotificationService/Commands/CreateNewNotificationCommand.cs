using eLib.Common.Notifications;
using MediatR;

namespace eLib.NotificationService.Commands;

public record CreateNewNotificationCommand(
    string Message,
    Guid UserId,
    ENotificationChannel Channel,
    string? Title,
    string? PhoneNumber,
    string? Email
    ) : IRequest;



public class CreateNewNotificationCommandHandler : IRequestHandler<CreateNewNotificationCommand>
{
    private readonly INotificationProcessor _notificationProcessor;

    public CreateNewNotificationCommandHandler(
        INotificationProcessor notificationProcessor)
    {
        _notificationProcessor = notificationProcessor;
    }

    public async Task Handle(CreateNewNotificationCommand request, CancellationToken cancellationToken)
    {
        await _notificationProcessor.ProcessAsync(
            ENotificationType.Custom,
            request.UserId,
            request.Channel,
            request.PhoneNumber,
            request.Email,
            request.Message,
            cancellationToken
        );
    }
}