using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record ReservationReturnedEvent(Reservation Reservation) : IDomainEvent;