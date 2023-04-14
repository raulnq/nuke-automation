using SqlKata.Execution;
using Core.Application;

namespace Core.Infrastructure;

public abstract class SqlKataAnyQueryRunner<TQuery> : SqlKataQueryRunner<TQuery, Any> where TQuery : Application.BaseQuery<Any>
{
    protected SqlKataAnyQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
    {
    }

    public override async Task<Any> Run(TQuery query)
    {
        var exist = await BuildQuery(query).ExistsAsync();

        return new Any() { Value = exist };
    }
}