using MediatR;
using Rebus.Handlers;
using Core.Application;
using Core.Events;

namespace Core.Infrastructure;

public abstract class BaseCommandDispatcher<TEvent> : IHandleMessages<TEvent> where TEvent : IEvent
{
    protected readonly IMediator _mediator;

    protected BaseCommandDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(TEvent message)
    {
        var command = await Map(message);

        if (command != null)
        {
            await _mediator.Send(command);
        }
    }

    protected abstract Task<ICommand> Map(TEvent @event);
}
