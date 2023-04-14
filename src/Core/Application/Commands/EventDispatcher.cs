using AutoMapper;
using MediatR;
using Core.Domain;
using Core.Events;

namespace Core.Application;

public class EventDispatcher<TDomainEvent, TEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
    where TEvent : IEvent
{
    private readonly IEventPublisher _publisher;
    private readonly IMapper _mapper;
    private readonly Func<TDomainEvent, bool>? _when;
    public EventDispatcher(IEventPublisher publisher, IMapper mapper, Func<TDomainEvent, bool>? when = null)
    {
        _publisher = publisher;
        _mapper = mapper;
        _when = when;
    }

    public Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        if (_when != null && !_when(notification))
        {
            return Task.CompletedTask;
        }

        var @event = _mapper.Map<TDomainEvent, TEvent>(notification);

        return _publisher.Publish(@event);
    }
}
