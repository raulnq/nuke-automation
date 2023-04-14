using Bogus;
using Shouldly;
using Core.Tests;
using WebAPI.Application.Products;
using Core.Application;

namespace Tests.Products
{
    public class ProductDsl
    {
        private readonly HttpDriver _httpDriver;

        private string _path = "products";

        public ProductDsl(HttpDriver httpDriver)
        {
            _httpDriver = httpDriver;
        }

        public async Task<(RegisterProduct.Command, RegisterProduct.Result?)> Register(Action<RegisterProduct.Command>? setup = null, string? errorDetail = null, IDictionary<string, string[]>? errors = null)
        {
            var faker = new Faker<RegisterProduct.Command>()
                .RuleFor(command => command.Description, faker => faker.Lorem.Sentence())
                .RuleFor(command => command.Name, faker => faker.Random.Guid().ToString());

            var request = faker.Generate();

            setup?.Invoke(request);

            var (status, result, error) = await _httpDriver.Post<RegisterProduct.Command, RegisterProduct.Result>(_path, request);

            (status, result, error).Check(errorDetail, errors: errors, successAssert: result =>
            {
                result.ProductId.ShouldNotBe(Guid.Empty);
            });

            return (request, result);
        }

        public async Task<(ListProducts.Query, ListResults<ListProducts.Result>?)> List(Action<ListProducts.Query>? setup = null, string? errorDetail = null)
        {
            var faker = new Faker<ListProducts.Query>()
                .RuleFor(command => command.Name, faker => faker.Lorem.Word());

            var request = faker.Generate();

            setup?.Invoke(request);

            var (status, result, error) = await _httpDriver.Get<ListProducts.Query, ListResults<ListProducts.Result>>(_path, request);

            (status, result, error).Check(errorDetail, successAssert: result =>
            {
                result.TotalCount.ShouldBeGreaterThan(0);
            });

            return (request, result);
        }
    }
}