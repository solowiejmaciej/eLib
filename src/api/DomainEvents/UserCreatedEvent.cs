using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record UserCreatedEvent(User User) : IDomainEvent;