using SqlKata.Execution;
using Core.Application;

namespace Core.Infrastructure;

public abstract class SqlKataCountQueryRunner<TQuery> : SqlKataQueryRunner<TQuery, Count> where TQuery : Application.BaseQuery<Count>
{
    protected SqlKataCountQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
    {
    }

    public override async Task<Count> Run(TQuery query)
    {
        var count = await BuildQuery(query).CountAsync<int>();

        return new Count() { Value = count };
    }
}
