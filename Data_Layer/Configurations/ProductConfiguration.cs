using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Data_Layer.Configurations;


public class ProductConfiguration :IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasMany(e => e.Images)
            .WithOne(e => e.Product)
            .HasForeignKey(e => e.ProductId);
        builder.HasMany(e=>e.ProductViews)
            .WithOne(e => e.Product)
            .HasForeignKey(e => e.ProductId);
        builder.HasMany(e => e.Comments)
            .WithOne(e => e.Product)
            .HasForeignKey(e => e.ProductId);

    }
    
}