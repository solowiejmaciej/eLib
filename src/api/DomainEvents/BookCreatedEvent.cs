using eLib.DAL.Entities;

namespace eLib.DomainEvents;

public record BookCreatedEvent(Book? Book) : IDomainEvent;