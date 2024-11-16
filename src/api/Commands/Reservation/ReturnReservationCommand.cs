using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Reservation;

public record ReturnReservationCommand(Guid ReservationId) : IResultCommand<Unit>;

public class ReturnReservationCommandValidator : AbstractValidator<ReturnReservationCommand>
{
    public ReturnReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty();
    }
}

public class ReturnReservationCommandHandler : IResultCommandHandler<ReturnReservationCommand, Unit>
{
    private readonly IReservationRepository _reservationRepository;

    public ReturnReservationCommandHandler(
        IReservationRepository reservationRepository
        )
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Unit, Error>> Handle(ReturnReservationCommand request, CancellationToken cancellationToken)
    {
       var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
       if (reservation is null)
           return ReservationError.NotFound;

       var error = reservation.Return();
       if (error is not null)
           return error;

       await _reservationRepository.UpdateAsync(reservation, cancellationToken);
       await _reservationRepository.SaveChangesAsync(cancellationToken);
       return Unit.Value;
    }
}