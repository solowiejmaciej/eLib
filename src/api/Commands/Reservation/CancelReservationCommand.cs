using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Commands.Reservation;

public record CancelReservationCommand(Guid ReservationId) : IResultCommand<Unit>;

public class CancelReservationCommandHandler : IResultCommandHandler<CancelReservationCommand, Unit>
{
    private readonly IReservationRepository _reservationRepository;

    public CancelReservationCommandHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Unit, Error>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
        if (reservation == null)
        {
            return ReservationError.NotFound;
        }

        var error = reservation.Cancel();

        if (error != null)
        {
            return error;
        }

        await _reservationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}