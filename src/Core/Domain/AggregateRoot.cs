namespace Core.Domain;

public class AggregateRoot : IAggregateRoot
{
    private readonly List<IDomainEvent> _events = new();

    public IReadOnlyList<IDomainEvent> Events => _events;

    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);

    public void ClearEvents() => _events.Clear();
}
