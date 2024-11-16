using eLib.DAL.Repositories;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationReturnedEventHandler : IDomainEventHandler<ReservationReturnedEvent>
{
    private readonly IBookRepository _bookRepository;

    public ReservationReturnedEventHandler(
        IBookRepository bookRepository
        )
    {
        _bookRepository = bookRepository;
    }

    public async Task Handle(ReservationReturnedEvent notification, CancellationToken cancellationToken)
    {
        var bookDetails = await _bookRepository.GetDetailsByBookIdAsync(notification.Reservation.BookId, cancellationToken);
        if (bookDetails is null)
            throw new InvalidOperationException("Book details not found");

        bookDetails.IncreaseAvailableCopies();

        await _bookRepository.SaveChangesAsync(cancellationToken);
    }
}