using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record ReservationCanceledEvent(Reservation Reservation) : IDomainEvent;