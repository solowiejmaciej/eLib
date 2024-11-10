using MediatR;

namespace eLib.DomainEvents.Handlers;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent;