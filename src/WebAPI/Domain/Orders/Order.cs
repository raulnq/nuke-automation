using Core.Domain;
using WebAPI.Domain.Clients;
using WebAPI.Domain.Products;

namespace WebAPI.Domain.Orders
{
    public record OrderId(Guid Value);

    public enum OrderStatus
    {
        Pending = 0, 
        Confirmed = 1,
        Completed = 2,
    }

    public class Order : AggregateRoot
    {
        public OrderId OrderId { get; private set; }
        public ClientId ClientId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public OrderStatus Status { get; private set; }
        public Money Total { get; private set; }
        public List<OrderItem> Items { get; private set; }
        public Order(OrderId orderId, ClientId clientId, string name, string email)
        {
            OrderId = orderId;
            ClientId = clientId;
            Name = name;
            Email = email;
            Status = OrderStatus.Pending;
        }

        public void Add(ProductId productId, Quantity quantity, Money price)
        {
            if(Items.Any(item=>item.ProductId==productId))
            {
                throw new DuplicatedException<OrderItem>();
            }

            Items.Add(new OrderItem(OrderId, productId, quantity, price));

            Total = Total + (price * quantity);
        }

        public void Confirm()
        {
            Status = OrderStatus.Confirmed;
        }

        private Order()
        {

        }
    }

    public class OrderItem
    {
        public OrderId OrderId { get; private set; }
        public ProductId ProductId { get; private set; }
        public Quantity Quantity { get; private set; }
        public Money Price { get; private set; }

        public OrderItem(OrderId orderId, ProductId productId, Quantity quantity, Money price)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        private OrderItem() 
        {
            
        }
    }
}
