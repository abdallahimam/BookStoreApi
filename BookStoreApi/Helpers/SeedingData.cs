using BookStoreApi.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace BookStoreApi.Helpers
{
    public static class SeedingData
    {
        public static async void SeedUsersAndRoles(this IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();

            string[] allRoles = new string[] { "SuperAdmin", "Admin", "User" };
            string[] adminRoles = new string[] { "Admin" };
            string[] superAdminRoles = new string[] { "SuperAdmin" };

            foreach (string role in allRoles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);

                if (!context.Roles.Any(r => r.Name == role))
                {
                    var identityRole = new IdentityRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                    await roleStore.CreateAsync(identityRole);
                }
            }

            var admin = new ApplicationUser
            {
                Fullname = "Admin",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "6668889999",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var superAdmin = new ApplicationUser
            {
                Fullname = "Super Admin",
                Email = "superadmin@admin.com",
                NormalizedEmail = "SUPERADMIN@ADMIN.COM",
                UserName = "SuperAdmin",
                NormalizedUserName = "SUPERADMIN",
                PhoneNumber = "2001016235698",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(u => u.UserName == admin.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(admin, "@Aa20130156");
                admin.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(admin);
            }

            await AssignRoles(serviceProvider, admin.Email, adminRoles);

            if (!context.Users.Any(u => u.UserName == superAdmin.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(superAdmin, "@Aa20130156");
                superAdmin.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(superAdmin);
            }

            await AssignRoles(serviceProvider, superAdmin.Email, superAdminRoles);

            await context.SaveChangesAsync();
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string email, string[] roles)
        {
            UserManager<ApplicationUser> _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.AddToRolesAsync(user, roles);
            
            return result;
        }
    }
}
