using eLib.Common.Dtos;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Reservation;

public record CreateReservationCommand(Guid BookId, Guid UserId, DateTime StartDate, DateTime? EndDate)
    : IResultCommand<Guid>;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
    }
}

public class CreateReservationCommandHandler : IResultCommandHandler<CreateReservationCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly IReservationRepository _reservationRepository;

    public CreateReservationCommandHandler(
        IBookRepository bookRepository,
        IReservationRepository reservationRepository)
    {
        _bookRepository = bookRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var bookDetails = await _bookRepository.GetDetailsByIdAsync(request.BookId, cancellationToken);
        if (bookDetails == null)
            return BookErrors.NotFound;

        var result = DAL.Entities.Reservation.Create(bookDetails, request.UserId, request.StartDate, request.EndDate);
        if (result.HasError())
            return result.Error;

        var reservation = result.Value;

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}