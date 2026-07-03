using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Layer.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(e => e.Orders)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);
        builder.HasMany(e=>e.Carts)
            .WithOne(e=>e.User)
            .HasForeignKey(e => e.UserId);
        builder.HasMany(e => e.Comments)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId);
        builder.HasMany(e => e.Roles)
            .WithMany(e => e.Users);
        builder.HasMany(e => e.Addresses)
            .WithOne(e => e.User);
        builder.HasMany(e => e.SnapShots)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}