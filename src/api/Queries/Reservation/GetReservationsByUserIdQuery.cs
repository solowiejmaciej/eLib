using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;

namespace eLib.Queries.Reservation;

public record GetReservationsByUserIdQuery(Guid UserId, PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<ReservationDto>>;

public class GetReservationsByUserIdQueryHandler : IResultQueryHandler<GetReservationsByUserIdQuery, PaginationResult<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsByUserIdQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<PaginationResult<ReservationDto>, Error>> Handle(GetReservationsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var userReservations = await _reservationRepository.GetPaginatedReservationsByUserId(request.UserId, request.PaginationParameters ,cancellationToken);
        return userReservations.MapToDto(x => x.MapToDto());
    }
}

