using eLib.DAL.Repositories;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationCanceledEventHandler : IDomainEventHandler<ReservationCanceledEvent>
{
    private readonly IBookRepository _bookRepository;

    public ReservationCanceledEventHandler(
        IBookRepository bookRepository
        )
    {
        _bookRepository = bookRepository;
    }

    public async Task Handle(ReservationCanceledEvent notification, CancellationToken cancellationToken)
    {
        var bookDetails = await _bookRepository.GetDetailsByBookIdAsync(notification.Reservation.BookId, cancellationToken);
        if (bookDetails is null)
            throw new InvalidOperationException("Book details not found");

        bookDetails.IncreaseAvailableCopies();

        await _bookRepository.SaveChangesAsync(cancellationToken);
    }
}