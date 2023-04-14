using Core.Domain;

namespace WebAPI.Domain.Products
{
    public record ProductId(Guid Value);

    public class Product : AggregateRoot
    {
        public ProductId ProductId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsEnabled { get; private set; }
        public Money Price { get; private set; }

        public Product(ProductId productId, string name, string? description, Money price, bool any)
        {
            if(any)
            {
                throw new DuplicatedException<Product>();
            }
            ProductId = productId;
            Name = name;
            Description = description;
            IsEnabled = false;
            Price = price;
        }

        private Product()
        {

        }

        public void Edit(string? description, Money price)
        {
            Description = description;
            Price = price;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable() 
        { 
            IsEnabled = false;
        }
    }

}
