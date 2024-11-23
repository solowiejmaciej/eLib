using eLib.Auth.Providers;
using eLib.Common;
using eLib.Common.Notifications;
using eLib.DAL.Repositories;
using eLib.Events.Events.Notifications;
using eLib.Events.Services;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationCreatedEventHandler : IDomainEventHandler<ReservationCreatedEvent>
{
    private readonly IBookRepository _bookRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUserRepository _userRepository;
    private readonly IUserInfoProvider _userInfoProvider;

    public ReservationCreatedEventHandler(
        IBookRepository bookRepository,
        IEventPublisher eventPublisher,
        IUserRepository userRepository,
        IUserInfoProvider userInfoProvider)
    {
        _bookRepository = bookRepository;
        _eventPublisher = eventPublisher;
        _userRepository = userRepository;
        _userInfoProvider = userInfoProvider;
    }

    public async Task Handle(ReservationCreatedEvent notification, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdWithDetailsAsync(notification.Reservation.BookId, cancellationToken);
        if (book is null)
            throw new InvalidOperationException("Book not found");

        var errors = book.Details.DecreaseAvailableCopies();
        if (errors is not null)
            throw new InvalidOperationException(errors.Message);

        await _bookRepository.SaveChangesAsync(cancellationToken);
        var userId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdWithDetailsAsync(userId, cancellationToken);
        var userInfo = user.MapToDto().MapToUserInfo();

        var associatedObjects = new List<SerializedObject>
        {
            new(notification.Reservation.MapToDto()),
            new(book.MapToDto())
        };

        await _eventPublisher.PublishAsync(new SendNotificationEvent(ENotificationType.ReservationCreated, userInfo, associatedObjects), cancellationToken);
    }
}