using Core.Domain;

namespace Core.Application;

public interface IDomainEventSource
{
    public IEnumerable<IDomainEvent> Get();
}
