using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record ReservationExtendedEvent(Reservation Reservation) : IDomainEvent;