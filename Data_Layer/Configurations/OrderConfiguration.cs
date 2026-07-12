using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Layer.Configurations;

public class OrderConfiguration :IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasMany(e => e.Items)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId);
        builder.HasOne(e => e.SnapShot)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.Payments)
            .WithOne(e => e.Order)
            .HasForeignKey(e => e.OrderId);
    }
}
public class OrderItemConfiguration :IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(x => x.Discount)
            .HasPrecision(5, 2);
    }
}