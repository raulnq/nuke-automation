using AutoMapper;
using MediatR;
using Core.Domain;

namespace Core.Application;

public class CommandDispatcher<TDomainEvent, TCommand> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
    where TCommand : ICommand
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly Func<TDomainEvent, bool>? _when;
    public CommandDispatcher(IMediator mediator, IMapper mapper, Func<TDomainEvent, bool>? when = null)
    {
        _mediator = mediator;
        _mapper = mapper;
        _when = when;
    }

    public Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        if (_when != null && !_when(notification))
        {
            return Task.CompletedTask;
        }

        var command = _mapper.Map<TDomainEvent, TCommand>(notification);

        return _mediator.Send(command, cancellationToken);
    }
}
