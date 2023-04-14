namespace Core.Application;

public interface IQueryRunner
{
}

public interface IQueryRunner<TQuery, TResult> : IQueryRunner where TQuery : BaseQuery<TResult>
{
    Task<TResult> Run(TQuery query);
}
