using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.Reservation;

public record GetAllReservationsQuery() : IResultQuery<IEnumerable<ReservationDto>>;

public class GetAllReservationsQueryHandler : IResultQueryHandler<GetAllReservationsQuery, IEnumerable<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetAllReservationsQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<IEnumerable<ReservationDto>, Error>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await _reservationRepository.GetAllAsync(cancellationToken);
        var reservationDtos = allReservations.Select(x => x.MapToDto());
        return Result<IEnumerable<ReservationDto>, Error>.FromEnumerable(reservationDtos);
    }
}
