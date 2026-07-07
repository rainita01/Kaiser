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
        builder.HasOne(e => e.Address)
            .WithMany(e => e.Orders)
            .HasForeignKey(e => e.AddressId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(e => e.SnapShot)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Payment).WithOne();
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