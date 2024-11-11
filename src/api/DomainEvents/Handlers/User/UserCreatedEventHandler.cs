namespace eLib.DomainEvents.Handlers.User;

public class UserCreatedEventHandler : IDomainEventHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"User created: {notification.User.Id}");
        return Task.CompletedTask;
    }
}