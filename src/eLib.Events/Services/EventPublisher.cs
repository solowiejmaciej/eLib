using eLib.Events.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace eLib.Events.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
        _logger.LogInformation($"Event published: {@event.GetType().Name}");
    }
}
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
}