using Core.Infrastructure;
using SqlKata;
using SqlKata.Execution;
using WebAPI.Application.Products;

namespace WebAPI.Infrastructure.Products;

public class ListProductsQueryRunner : SqlKataListQueryRunner<ListProducts.Query, ListProducts.Result>
{
    private readonly Table _products;

    public ListProductsQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
    {
        _products = new Table(dbSchema.Name, Tables.Products);
    }

    protected override Query BuildQuery(ListProducts.Query query)
    {
        var statement = QueryFactory.Query(_products);

        if (!string.IsNullOrEmpty(query.Name))
        {
            statement = statement.WhereLike(_products.Field("Name"), $"%{query.Name}%");
        }

        return statement;
    }
}