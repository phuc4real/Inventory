using Inventory.Core.Constants;
using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Inventory.API.Extensions
{
    public class DbSeeder
    {
        public static async Task Initialize(AppDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            #region Seed User

            var userStore = new UserStore<AppUser>(dbContext);
            var hasher = new PasswordHasher<AppUser>();

            if (!dbContext.Users.Any(x=>x.Email == "phuc@local.com"))
            {

                AppUser user = new()
                {
                    UserName = "phucle",
                    NormalizedUserName = "PHUCLE",
                    Email = "phuc@local.com",
                    NormalizedEmail = "PHUC@LOCAL.COM",
                    FirstName = "Phuc",
                    LastName = "Le",
                };
                user.PasswordHash = hasher.HashPassword(user, "123456@@");

                var result = await userStore.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userStore.AddToRoleAsync(user, InventoryRoles.SuperAdmin);
                }
            }

            #endregion
        }
    }
}
