using Data_Layer.Configurations;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Context;

public class Context(DbContextOptions options) 
    :  IdentityDbContext<User, Role, string,IdentityUserClaim<string>,IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
    IdentityUserToken<string>>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ProductView> ProductViews { get; set; }    
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        base.OnModelCreating(builder);
    }
}