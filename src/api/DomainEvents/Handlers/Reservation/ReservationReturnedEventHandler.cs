using eLib.Auth.Providers;
using eLib.Common;
using eLib.Common.Notifications;
using eLib.DAL.Repositories;
using eLib.Events.Events.Notifications;
using eLib.Events.Services;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationReturnedEventHandler : IDomainEventHandler<ReservationReturnedEvent>
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserInfoProvider _userInfoProvider;
    private readonly IEventPublisher _eventPublisher;

    public ReservationReturnedEventHandler(
        IBookRepository bookRepository,
        IUserInfoProvider userInfoProvider,
        IEventPublisher eventPublisher)
    {
        _bookRepository = bookRepository;
        _userInfoProvider = userInfoProvider;
        _eventPublisher = eventPublisher;
    }

    public async Task Handle(ReservationReturnedEvent notification, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdWithDetailsAsync(notification.Reservation.BookId, cancellationToken);
        if (book is null)
            throw new InvalidOperationException("Book not found");

        book.Details.IncreaseAvailableCopies();

        var userInfo = _userInfoProvider.GetCurrentUser();

        var associatedObjects = new List<SerializedObject>
        {
            new(notification.Reservation.MapToDto()),
            new(book.MapToDto())
        };

        await _bookRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new SendNotificationEvent(ENotificationType.ReservationCanceled, userInfo, associatedObjects), cancellationToken);
    }
}