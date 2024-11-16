using eLib.DAL.Repositories;

namespace eLib.DomainEvents.Handlers.Reservation;

public class ReservationCreatedEventHandler : IDomainEventHandler<ReservationCreatedEvent>
{
    private readonly IBookRepository _bookRepository;

    public ReservationCreatedEventHandler(
        IBookRepository bookRepository
        )
    {
        _bookRepository = bookRepository;
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
    }
}