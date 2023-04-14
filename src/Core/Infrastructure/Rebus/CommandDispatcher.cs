using AutoMapper;
using MediatR;
using Rebus.Handlers;
using Core.Application;
using Core.Events;

namespace Core.Infrastructure;

public class CommandDispatcher<TEvent, TCommmand> : IHandleMessages<TEvent>
    where TEvent : IEvent
    where TCommmand : ICommand
{
    private readonly IMediator _mediator;

    private readonly IMapper _mapper;

    private readonly Func<TEvent, bool>? _when;

    public CommandDispatcher(IMediator mediator, IMapper mapper, Func<TEvent, bool>? when)
    {
        _mediator = mediator;
        _mapper = mapper;
        _when = when;
    }

    public Task Handle(TEvent message)
    {
        if (_when != null && !_when(message))
        {
            return Task.CompletedTask;
        }

        var command = _mapper.Map<TEvent, TCommmand>(message);

        return _mediator.Send(command);
    }
}

public class CommandDispatcher<TCommmand> : IHandleMessages<TCommmand>
where TCommmand : ICommand
{
    private readonly IMediator _mediator;

    public CommandDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(TCommmand message)
    {
        return _mediator.Send(message);
    }
}
