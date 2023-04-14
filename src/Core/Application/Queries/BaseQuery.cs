using MediatR;

namespace Core.Application;

public class BaseQuery<TResult> : IQuery, IRequest<TResult>
{
}
