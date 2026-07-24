using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core_Layer.Services.Seed;

public class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            await roleManager.CreateAsync(new Role(){Name = "admin"});
        }
        var admin = await userManager.FindByNameAsync("Sysadmin");

        if (admin is null)
        {
            admin = new User
            {
                UserName = "Sysadmin",
                Email = "admin@test.com",
                EmailConfirmed = true,
            };

            await userManager.CreateAsync(admin, "2026admin1405");
            await userManager.AddToRoleAsync(admin, "admin");
        }

    }
}