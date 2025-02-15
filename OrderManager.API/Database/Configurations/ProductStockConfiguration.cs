using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OrderManager.API.Models;

namespace OrderManager.API.Database.Configurations
{
    public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
    {
        public void Configure(EntityTypeBuilder<ProductStock> builder)
        {
            builder.HasKey(ps => ps.ProductId);
            builder.Property(ps => ps.Quantity).IsRequired();

            builder.HasOne(ps => ps.Product)
                   .WithOne(p => p.ProductStock)
                   .HasForeignKey<ProductStock>(ps => ps.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
