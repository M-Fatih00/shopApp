using Microsoft.AspNetCore.Identity;

namespace AspNetAPI.Data;

public static class SeedDatabase
{
    public static async Task SeedRolesAndUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        // Roller
        string[] roles = { "Admin", "Customer" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole { Name = roleName });
            }
        }

        // Admin kullanıcısı
        if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
        {
            var adminUser = new AppUser
            {
                UserName = "admin_admin",
                Email = "admin@gmail.com",
                AdSoyad = "admin",
                DateAdded = DateTime.Now
            };

            await userManager.CreateAsync(adminUser, "123456");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Customer kullanıcısı
        if (await userManager.FindByEmailAsync("customer@gmail.com") == null)
        {
            var customerUser = new AppUser
            {
                UserName = "customer",
                Email = "customer@gmail.com",
                AdSoyad = "customer",
                DateAdded = DateTime.Now
            };

            await userManager.CreateAsync(customerUser, "123456");
            await userManager.AddToRoleAsync(customerUser, "Customer");
        }
    }
}