using Identity.WebAPI.Models;
using Identity.WebAPI.Options;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebAPI.Context
{
    public class IdentitySeedData
    {
        public static async void CreateAdminUserAndRoles(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new AppRole() { Name = UserRoles.Admin });
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new AppRole() { Name = UserRoles.User });

                var user = await userManager.FindByEmailAsync("admin@admin.com");

                string password = "admin1234";

                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = "admin@admin.com",
                        FirstName = "Admin",
                        LastName = "Admin",
                        Email = "admin@admin.com",
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
            }
        }
    }
}
