using eLib.Auth.Providers;
using eLib.Common;
using eLib.Common.Notifications;
using eLib.Events.Events.Notifications;
using eLib.Events.Services;

namespace eLib.DomainEvents.Handlers.User;

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
{
    private readonly IEventPublisher _eventPublisher;

    public UserCreatedEventHandler(
        IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var userInfo = notification.User.MapToDto().MapToUserInfo();

        var associatedObjects = new List<SerializedObject>
        {
            new(userInfo)
        };

        return _eventPublisher.PublishAsync(new SendNotificationEvent(ENotificationType.AccountCreated, userInfo, associatedObjects), cancellationToken);
    }
}