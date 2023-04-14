using Core.Application;
using Core.Domain;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using WebAPI.Domain.Products;

namespace WebAPI.Application.Products;

public class RegisterProduct
{
    public class Command : BaseCommand<Result>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        [JsonIgnore]
        public bool Any { get; set; }
    }

    public class Result
    {
        public Guid ProductId { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(command => command.Name).MaximumLength(255).NotEmpty();
            RuleFor(command => command.Description).MaximumLength(4000);
        }
    }

    public class CommandHandler : IRequestHandler<Command, Result>
    {
        private readonly IRepository<Product> _productRepository;

        public CommandHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var product = new Product(new ProductId(Guid.NewGuid()), command.Name!, command.Description, Money.From(command.Price), command.Any);

            _productRepository.Add(product);

            return Task.FromResult(new Result() { ProductId = product.ProductId.Value });
        }
    }
}
