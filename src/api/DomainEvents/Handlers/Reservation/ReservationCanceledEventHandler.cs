using eLib.Auth.Providers;
using eLib.Common;
using eLib.Common.Notifications;
using eLib.DAL.Repositories;
using eLib.Events.Events.Notifications;
using eLib.Events.Services;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationCanceledEventHandler : IDomainEventHandler<ReservationCanceledEvent>
{
    private readonly IBookRepository _bookRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUserInfoProvider _userInfoProvider;

    public ReservationCanceledEventHandler(
        IBookRepository bookRepository,
        IEventPublisher eventPublisher,
        IUserInfoProvider userInfoProvider)
    {
        _bookRepository = bookRepository;
        _eventPublisher = eventPublisher;
        _userInfoProvider = userInfoProvider;
    }

    public async Task Handle(ReservationCanceledEvent notification, CancellationToken cancellationToken)
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