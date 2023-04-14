namespace Core.Domain;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> Events { get; }

    public void ClearEvents();
}
