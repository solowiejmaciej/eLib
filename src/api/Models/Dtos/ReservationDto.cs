using eLib.DAL.Entities.Enums;

namespace eLib.Models.Dtos;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public DateTime? ExtendedAt { get; set; }
    public EReservationStatus Status { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsReturned { get; set; }
    public bool IsExtended { get; set; }
    public bool IsActive { get; set; }
}