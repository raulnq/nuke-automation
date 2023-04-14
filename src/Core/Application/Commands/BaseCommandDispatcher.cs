using MediatR;
using Core.Domain;

namespace Core.Application;

public abstract class BaseCommandDispatcher<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    protected readonly IMediator _mediator;

    protected BaseCommandDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        var command = await Map(notification);

        if (command != null)
        {
            await _mediator.Send(command, cancellationToken);
        }
    }

    protected abstract Task<ICommand> Map(TDomainEvent @event);
}
