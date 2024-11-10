using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record AuthorCreatedEvent(Author Author) : IDomainEvent;
