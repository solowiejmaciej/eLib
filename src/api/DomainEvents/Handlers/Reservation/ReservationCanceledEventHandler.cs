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
    private readonly IUserRepository _userRepository;

    public ReservationCanceledEventHandler(
        IBookRepository bookRepository,
        IEventPublisher eventPublisher,
        IUserInfoProvider userInfoProvider,
        IUserRepository userRepository)
    {
        _bookRepository = bookRepository;
        _eventPublisher = eventPublisher;
        _userInfoProvider = userInfoProvider;
        _userRepository = userRepository;
    }

    public async Task Handle(ReservationCanceledEvent notification, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdWithDetailsAsync(notification.Reservation.BookId, cancellationToken);
        if (book is null)
            throw new InvalidOperationException("Book not found");

        book.Details.IncreaseAvailableCopies();

        var userId = _userInfoProvider.GetCurrentUserID();
        var user = await _userRepository.GetByIdWithDetailsAsync(userId, cancellationToken);
        var userInfo = user.MapToDto().MapToUserInfo();

        var associatedObjects = new List<SerializedObject>
        {
            new(notification.Reservation.MapToDto()),
            new(book.MapToDto()),
            new(userInfo)
        };

        await _bookRepository.SaveChangesAsync(cancellationToken);

        await _eventPublisher.PublishAsync(new SendNotificationEvent(ENotificationType.ReservationCanceled, userInfo, associatedObjects), cancellationToken);
    }
}