using MediatR;

namespace Core.Application;

public class PublishDomainEventBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IDomainEventSource _source;
    private readonly IMediator _mediator;

    public PublishDomainEventBehavior(
        IDomainEventSource source,
        IMediator mediator)
    {
        _source = source;
        _mediator = mediator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        foreach (var @event in _source.Get())
        {
            await _mediator.Publish(@event);
        }

        return response;
    }
}
