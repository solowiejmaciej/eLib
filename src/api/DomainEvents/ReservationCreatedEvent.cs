using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record ReservationCreatedEvent(Reservation Reservation) : IDomainEvent;