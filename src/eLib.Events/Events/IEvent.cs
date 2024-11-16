namespace eLib.Events.Events;

public interface IEvent
{
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public EEventType EventType { get; }
}