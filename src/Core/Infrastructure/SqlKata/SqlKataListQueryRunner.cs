using SqlKata;
using SqlKata.Execution;
using Core.Application;

namespace Core.Infrastructure;

public abstract class SqlKataListQueryRunner<TQuery, TResult> : SqlKataQueryRunner<TQuery, ListResults<TResult>> where TQuery : ListQuery<TResult>
{
    protected SqlKataListQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
    {
    }

    public override Task<ListResults<TResult>> Run(TQuery query)
    {
        var statement = BuildQuery(query);

        return ToListResult(statement, query);
    }

    public async Task<ListResults<TResult>> ToListResult(Query statement, TQuery query)
    {
        int count = await Count(statement);

        if (query.OrderBy?.Length > 0)
        {
            var orderBy = NormalizeOrderBy(query.OrderBy);

            if (query.Ascending)
            {
                statement = statement.OrderBy(orderBy);
            }
            else
            {
                statement = statement.OrderByDesc(orderBy);
            }
        }

        var result = await statement.ForPage(query.Page, query.PageSize).GetAsync<TResult>();

        return new ListResults<TResult>(query, count, result);
    }

    protected virtual Task<int> Count(Query statement)
    {
        return statement.Clone().CountAsync<int>();
    }

    protected virtual string[] NormalizeOrderBy(string[] orderBy) => orderBy;
}

