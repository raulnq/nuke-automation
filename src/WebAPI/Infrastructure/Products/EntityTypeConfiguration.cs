using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WebAPI.Domain.Products;
using Core.Domain;

namespace WebAPI.Infrastructure.Products;

public class EntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .ToTable(Tables.Products);

        builder
            .HasKey(product => product.ProductId);

        builder
            .Property(product => product.ProductId)
            .HasConversion(productId => productId.Value, value => new ProductId(value));

        builder
            .Property(product => product.Price)
            .HasConversion(price => price.Value, value => new Money(value))
            .HasColumnType("decimal(19,4)");
    }
}
