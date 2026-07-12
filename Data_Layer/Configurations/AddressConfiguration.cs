using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Layer.Configurations;

public class AddressConfiguration :IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasOne(e => e.City)
            .WithMany(e => e.Addresses)
            .HasForeignKey(e => e.CityId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(e => e.Province)
            .WithMany(e => e.Addresses)
            .HasForeignKey(e => e.ProvinceId)
            .OnDelete(DeleteBehavior.NoAction);
        
    }
        
}

public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.HasMany(e => e.Cities)
            .WithOne(e => e.Province)
            .HasForeignKey(e => e.ProvinceId);
    }
}