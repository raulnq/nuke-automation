using MediatR;

namespace Core.Application;

public class QueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : BaseQuery<TResult>
{
    private readonly IQueryRunner<TQuery, TResult> _queryRunner;

    public QueryHandler(IQueryRunner<TQuery, TResult> queryRunner)
    {
        _queryRunner = queryRunner;
    }

    public virtual Task<TResult> Handle(TQuery request, CancellationToken cancellationToken)
    {
        return _queryRunner.Run(request);
    }
}
