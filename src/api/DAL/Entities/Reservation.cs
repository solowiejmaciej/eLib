using eLib.DAL.Entities.Enums;
using eLib.DomainEvents;
using eLib.Models.Dtos;
using eLib.Models.Results;
using eLib.Models.Results.Base;

namespace eLib.DAL.Entities;

public class Reservation : AggregateRoot
{
    private Reservation() : base(Guid.NewGuid()) { }

    private Reservation(Guid bookId, Guid userId, DateTime startDate, DateTime? endDate) : base(Guid.NewGuid())
    {
        BookId = bookId;
        UserId = userId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public DateTime? ExtendedAt { get; private set; }
    public EReservationStatus Status { get; private set; }
    public bool IsOverdue =>  EndDate <= DateTime.UtcNow;
    public bool IsReturned => ReturnedAt.HasValue;
    public bool IsCanceled => CanceledAt.HasValue;
    public bool IsExtended => ExtendedAt.HasValue;
    public bool IsActive => !IsReturned && !IsCanceled;

    public Book Book { get; private set; }
    public User User { get; private set; }

    public static Result<Reservation, Error> Create(BookDetails bookDetails, Guid userId, DateTime startDate, DateTime? endDate)
    {
        if (bookDetails.Quantity == 0)
            return ReservationError.BookNotAvailable;

        if (endDate < startDate)
            return ReservationError.InvalidEndDate;


        var reservation = new Reservation(bookDetails.BookId, userId, startDate, endDate);
        reservation.Status = EReservationStatus.Created;
        reservation.RaiseDomainEvent(new ReservationCreatedEvent(reservation));
        return reservation;
    }

    public Error? Return()
    {
        var error = CheckReservationConditions();
        if(error is not null)
            return error;

        Status = EReservationStatus.Returned;
        ReturnedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReservationReturnedEvent(this));

        return null;
    }

    public Error? Cancel()
    {
        var error = CheckReservationConditions();
        if(error is not null)
            return error;

        Status = EReservationStatus.Canceled;
        CanceledAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReservationCanceledEvent(this));

        return null;
    }

    public Error? Extend(DateTime endDate)
    {
        var error = CheckReservationConditions();
        if(error is not null)
            return error;

        EndDate = endDate;
        ExtendedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReservationExtendedEvent(this));

        return null;
    }

    private Error? CheckReservationConditions()
    {
        if (IsReturned)
            return ReservationError.Returned;

        if (IsCanceled)
            return ReservationError.Canceled;

        return null;
    }

    public ReservationDto MapToDto()
        => new()
        {
             Id = Id,
             UserId = UserId,
             BookId = BookId,
             StartDate = StartDate,
             EndDate = EndDate,
             ReturnedAt = ReturnedAt,
             CanceledAt = CanceledAt,
             ExtendedAt = ExtendedAt,
             Status = Status,
             IsOverdue = IsOverdue,
             IsReturned = IsReturned,
             IsExtended = IsExtended,
             IsActive = IsActive
         };
}