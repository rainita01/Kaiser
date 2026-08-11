using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Layer.Configurations;

public class SnapShotConfiguration :IEntityTypeConfiguration<SnapShot>
{
    public void Configure(EntityTypeBuilder<SnapShot> builder)
    {
        builder.HasMany(e => e.Items)
            .WithOne(e => e.SnapShot)
            .HasForeignKey(e => e.SnapShotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
public class SnapShotItemConfiguration : IEntityTypeConfiguration<SnapShotItem>
{
    public void Configure(EntityTypeBuilder<SnapShotItem> builder)
    {
       builder.Property(x => x.Discount)
           .HasPrecision(5, 2);

    }
}