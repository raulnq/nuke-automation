using Core.Events;

namespace Core.Application;

public interface IEventPublisher
{
    Task Publish(IEvent @event, IDictionary<string, string>? headers = null);
}
