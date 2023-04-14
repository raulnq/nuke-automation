using Core.Domain;

namespace WebAPI.Domain.Clients
{
    public record ClientId(Guid Value);

    public class Client : AggregateRoot
    {
        public ClientId ClientId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        public Client(ClientId clientId, string name, string email, string phoneNumber)
        {
            ClientId = clientId;
            Name = name;
            Email = email; 
            PhoneNumber= phoneNumber;
        }

        private Client() 
        {
            
        }
    }
}
