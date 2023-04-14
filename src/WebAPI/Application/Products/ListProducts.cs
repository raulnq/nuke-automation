using Core.Application;
using FluentValidation;

namespace WebAPI.Application.Products;

public static class ListProducts
{
    public class Query : ListQuery<Result>
    {
        public string? Name { get; set; }
    }

    public class Result
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsEnabled { get; set; }
        public decimal Price { get; set; }
    }

    public class QueryValidator : AbstractValidator<Query>
    {
    }
}