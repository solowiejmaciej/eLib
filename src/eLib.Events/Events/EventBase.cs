namespace eLib.Events.Events;

public class EventBase : IEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public EEventType EventType { get; set; }
}