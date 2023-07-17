using Inventory.Core.Common;
using Inventory.Repository.DbContext;
using Inventory.Repository.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.DataSeed
{
    public static class ModelBuilderExtension
    {
        public static void SeedingData(this ModelBuilder builder)
        {
            #region User
            //Add Role
            builder.Entity<IdentityRole>()
            .HasData(new IdentityRole
            {
                Id = "46a4f2b7-2a9e-4977-ae32-e0e5793e6267",
                Name = InventoryRoles.Employee,
                NormalizedName = InventoryRoles.Employee.ToUpper(),
            });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = "f8b59b69-fabb-4386-948e-5fb7054ffff4",
                    Name = InventoryRoles.PM,
                    NormalizedName = InventoryRoles.PM.ToUpper(),
                });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = "4e5e4a2b-9b92-40fa-87f2-1fefc574336b",
                    Name = InventoryRoles.IM,
                    NormalizedName = InventoryRoles.IM.ToUpper(),
                });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = "fc2a7273-a3c2-47be-bc55-aab11097e09a",
                    Name = InventoryRoles.Admin,
                    NormalizedName = InventoryRoles.Admin.ToUpper(),
                });

            //Add default admin
            AppUser admin = new()
            {
                Id = "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@local.com",
                NormalizedEmail = "ADMIN@LOCAL.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var hasher = new PasswordHasher<AppUser>();
            admin.PasswordHash = hasher.HashPassword(admin, "tH1s1S@dM1nY0un3v3rKn0W");

            builder.Entity<AppUser>()
                .HasData(admin);

            builder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>
                {
                    RoleId = "fc2a7273-a3c2-47be-bc55-aab11097e09a",
                    UserId = "d2f7a36c-d4a6-43db-8fe9-74598da4c352"
                });
            #endregion
        }
    }
}
