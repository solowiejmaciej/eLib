using eLib.Common.Dtos;
using eLib.DAL.Pagination;
using eLib.DAL.Repositories;
using eLib.Models.Results.Base;
using MediatR;

namespace eLib.Queries.Reservation;

public record GetAllReservationsQuery(PaginationParameters PaginationParameters) : IResultQuery<PaginationResult<ReservationDto>>;

public class GetAllReservationsQueryHandler : IResultQueryHandler<GetAllReservationsQuery, PaginationResult<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetAllReservationsQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<PaginationResult<ReservationDto>, Error>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
    {
        var allReservations = await _reservationRepository.GetAllPaginatedAsync(request.PaginationParameters, cancellationToken);
        return allReservations.MapToDto(x => x.MapToDto());
    }
}
