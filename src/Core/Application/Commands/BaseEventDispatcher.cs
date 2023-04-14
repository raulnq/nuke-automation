using MediatR;
using Core.Domain;
using Core.Events;

namespace Core.Application;

public abstract class BaseEventDispatcher<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    private readonly IEventPublisher _publisher;

    protected BaseEventDispatcher(IEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        var @event = await Map(notification);

        if (@event != null)
        {
            await _publisher.Publish(@event);
        }
    }

    protected abstract Task<IEvent> Map(TDomainEvent @event);
}
