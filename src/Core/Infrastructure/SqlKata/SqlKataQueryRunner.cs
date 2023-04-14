using SqlKata;
using SqlKata.Execution;
using Core.Application;
using Core.Domain;

namespace Core.Infrastructure;

public abstract class SqlKataQueryRunner<TQuery, TResult> : IQueryRunner<TQuery, TResult> where TQuery : Application.BaseQuery<TResult>
{
    protected readonly QueryFactory QueryFactory;

    protected readonly DbSchema DbSchema;

    protected SqlKataQueryRunner(QueryFactory queryFactory, DbSchema dbSchema)
    {
        QueryFactory = queryFactory;
        DbSchema = dbSchema;
    }

    protected abstract Query BuildQuery(TQuery query);

    public virtual async Task<TResult> Run(TQuery query)
    {
        var result = await BuildQuery(query).FirstOrDefaultAsync<TResult>();

        if (result == null)
        {
            throw new NotFoundException();
        }

        return result;
    }
}
