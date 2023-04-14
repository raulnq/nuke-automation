namespace Core.Events;

public class @Event : IEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}