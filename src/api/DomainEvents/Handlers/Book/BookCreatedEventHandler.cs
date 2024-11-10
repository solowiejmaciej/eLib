namespace eLib.DomainEvents.Handlers.Book;

public class BookCreatedEventHandler : IDomainEventHandler<BookCreatedEvent>
{
    public Task Handle(BookCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Book created: {notification.Book.Id}");
        return Task.CompletedTask;
    }
}