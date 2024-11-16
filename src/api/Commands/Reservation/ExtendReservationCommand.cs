using System.Text.Json.Serialization;
using eLib.DAL.Repositories;
using eLib.Models.Results;
using eLib.Models.Results.Base;
using FluentValidation;
using MediatR;

namespace eLib.Commands.Reservation;

public record ExtendReservationCommand : IResultCommand<Unit>
{
    public ExtendReservationCommand(Guid id, DateTime newEndDate)
    {
        NewEndDate = newEndDate;
        Id = id;
    }

    [JsonIgnore]
    public Guid Id { get; set; }
    public DateTime NewEndDate { get; set; }
}

public class ExtendReservationCommandValidator : AbstractValidator<ExtendReservationCommand>
{
    public ExtendReservationCommandValidator()
    {
        RuleFor(x => x.NewEndDate).NotEmpty();
    }
}

public class ExtendReservationCommandHandler : IResultCommandHandler<ExtendReservationCommand, Unit>
{
    private readonly IReservationRepository _reservationRepository;

    public ExtendReservationCommandHandler(
        IReservationRepository reservationRepository
    )
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Unit, Error>> Handle(ExtendReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id, cancellationToken);

        if (reservation is null)
            return ReservationError.NotFound;

        var error = reservation.Extend(request.NewEndDate);
        if (error is not null)
            return error;

        await _reservationRepository.UpdateAsync(reservation, cancellationToken);
        await _reservationRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}