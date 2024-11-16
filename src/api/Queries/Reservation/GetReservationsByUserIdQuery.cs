using eLib.DAL.Repositories;
using eLib.Models.Dtos;
using eLib.Models.Results.Base;

namespace eLib.Queries.Reservation;

public record GetReservationsByUserIdQuery(Guid UserId) : IResultQuery<IEnumerable<ReservationDto>>;

public class GetReservationsByUserIdQueryHandler : IResultQueryHandler<GetReservationsByUserIdQuery, IEnumerable<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsByUserIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<IEnumerable<ReservationDto>, Error>> Handle(GetReservationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userReservations = await _reservationRepository.GetReservationsByUserId(request.UserId, cancellationToken);
        var dtos = userReservations.Select(x => x.MapToDto());
        return Result<IEnumerable<ReservationDto>, Error>.FromEnumerable(dtos);
    }
}

