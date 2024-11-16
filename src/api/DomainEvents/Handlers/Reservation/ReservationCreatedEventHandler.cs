using eLib.DAL.Repositories;
using eLib.Events;
using eLib.Events.Events;
using eLib.Events.Events.Notifications;
using eLib.Events.Services;
using eLib.Providers;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationCreatedEventHandler : IDomainEventHandler<ReservationCreatedEvent>
{
    private readonly IBookRepository _bookRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUserInfoProvider _userInfoProvider;

    public ReservationCreatedEventHandler(
        IBookRepository bookRepository,
        IEventPublisher eventPublisher, IUserInfoProvider userInfoProvider)
    {
        _bookRepository = bookRepository;
        _eventPublisher = eventPublisher;
        _userInfoProvider = userInfoProvider;
    }

    public async Task Handle(ReservationCreatedEvent notification, CancellationToken cancellationToken)
    {
        var bookDetails = await _bookRepository.GetDetailsByBookIdAsync(notification.Reservation.BookId, cancellationToken);
        if (bookDetails is null)
            throw new InvalidOperationException("Book details not found");

        var errors = bookDetails.DecreaseAvailableCopies();
        if (errors is not null)
            throw new InvalidOperationException(errors.Message);

        await _bookRepository.SaveChangesAsync(cancellationToken);
        var userInfo = _userInfoProvider.GetCurrentUser();
        await _eventPublisher.PublishAsync(new SendNotificationEvent(ENotificationType.ReservationCreated, userInfo), cancellationToken);
    }
}