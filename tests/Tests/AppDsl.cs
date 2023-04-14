using Core.Infrastructure;
using Core.Tests;
using Tests.Products;

namespace Tests
{
    internal class AppDsl : IAsyncDisposable
    {
        private readonly ApplicationFactory _applicationFactory;

        public AppDsl()
        {
            var clock = new FixedClock(DateTimeOffset.UtcNow);

            _applicationFactory = new ApplicationFactory
            {
                Clock = clock,
                ApiKeys = new ApiKeySettings { { "fake-api-key", "Admin" } }
            };

            var httpDriver = new HttpDriver(_applicationFactory, "fake-api-key");

            Product = new ProductDsl(httpDriver);
        }

        public ProductDsl Product { get; }

        public ValueTask DisposeAsync()
        {
            return _applicationFactory.DisposeAsync();
        }
    }
}