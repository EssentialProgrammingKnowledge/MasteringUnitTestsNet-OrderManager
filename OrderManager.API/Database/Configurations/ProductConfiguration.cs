using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderManager.API.Models;

namespace OrderManager.API.Database.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Price).HasPrecision(18, 4);

            builder.HasOne(p => p.ProductStock)
                   .WithOne(ps => ps.Product)
                   .HasForeignKey<ProductStock>(ps => ps.ProductId)
                   .IsRequired(false);

            builder.Navigation(p => p.ProductStock)
                   .AutoInclude();
        }
    }
}
