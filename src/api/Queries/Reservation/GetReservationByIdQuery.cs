using eLib.Common.Dtos;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Queries.Reservation;

public record GetReservationByIdQuery(Guid Id) : IResultQuery<ReservationDto?>;

public class GetReservationByIdQueryValidator : AbstractValidator<GetReservationByIdQuery>
{
    public GetReservationByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetReservationByIdQueryHandler : IResultQueryHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByIdQueryHandler(
        IReservationRepository reservationRepository
        )
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<ReservationDto?, Error>> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (reservation == null)
        {
            return ReservationError.NotFound;
        }

        return reservation.MapToDto();
    }
}