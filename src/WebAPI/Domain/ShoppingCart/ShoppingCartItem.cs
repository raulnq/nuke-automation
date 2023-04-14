using Core.Domain;
using WebAPI.Domain.Clients;
using WebAPI.Domain.Products;

namespace WebAPI.Domain.ShoppingCart
{
    public record ShoppingCartItemId(Guid Value);

    public class ShoppingCartItem : AggregateRoot
    {
        public ShoppingCartItemId ShoppingCartItemId { get; private set; }
        public ClientId ClientId { get; private set; }
        public ProductId ProductId { get; private set; }
        public Quantity Quantity { get; private set; }

        public ShoppingCartItem(ShoppingCartItemId shoppingCartItemId, ClientId clientId, ProductId productId, Quantity quantity, bool any)
        {
            if (any)
            {
                throw new DuplicatedException<ShoppingCartItem>();
            }
            ShoppingCartItemId = shoppingCartItemId;
            ClientId = clientId;
            ProductId = productId;
            Quantity = quantity;
        }

        private ShoppingCartItem()
        {

        }
    }
}
