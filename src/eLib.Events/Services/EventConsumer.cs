using eLib.Events.Events;
using MassTransit;

namespace eLib.Events.Services;

public interface IEventConsumer<TEvent> : IConsumer<TEvent> where TEvent : class, IEvent
{
}