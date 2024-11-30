using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands.Book;

public record DeleteBookCommand(Guid Id) : IResultCommand<Unit>;

public class DeleteBookCommandHandler : IResultCommandHandler<DeleteBookCommand, Unit>
{
    private readonly IBookRepository _bookRepository;
    private readonly IReservationRepository _reservationRepository;

    public DeleteBookCommandHandler(
        IBookRepository bookRepository,
        IReservationRepository reservationRepository)
    {
        _bookRepository = bookRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Unit, Error>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book == null)
        {
            return BookErrors.NotFound;
        }

        var hasAnyReservations = await _reservationRepository.HasAnyReservations(book.Id, cancellationToken);
        if (hasAnyReservations)
        {
            return BookErrors.HasReservations;
        }


        await _bookRepository.DeleteAsync(book.Id, cancellationToken);
        await _bookRepository.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}