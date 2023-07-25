using Inventory.Core.Common;
using Inventory.Repository.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                Name = InventoryRoles.NormalUser,
                NormalizedName = InventoryRoles.NormalUser.ToUpper(),
            });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = "f8b59b69-fabb-4386-948e-5fb7054ffff4",
                    Name = InventoryRoles.TeamLeader,
                    NormalizedName = InventoryRoles.TeamLeader.ToUpper(),
                });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = "4e5e4a2b-9b92-40fa-87f2-1fefc574336b",
                    Name = InventoryRoles.Admin,
                    NormalizedName = InventoryRoles.Admin.ToUpper(),
                });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole
                {
                    Id = "fc2a7273-a3c2-47be-bc55-aab11097e09a",
                    Name = InventoryRoles.SuperAdmin,
                    NormalizedName = InventoryRoles.SuperAdmin.ToUpper(),
                });

            //Add default SuperAdmin
            AppUserEntity sa = new()
            {
                Id = "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                UserName = "superadmin",
                NormalizedUserName = "SUPERADMIN",
                Email = "sa@local.com",
                NormalizedEmail = "SA@LOCAL.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var hasher = new PasswordHasher<AppUserEntity>();
            sa.PasswordHash = hasher.HashPassword(sa, "123456@@");

            builder.Entity<AppUserEntity>()
                .HasData(sa);

            builder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>
                {
                    RoleId = "fc2a7273-a3c2-47be-bc55-aab11097e09a",
                    UserId = "d2f7a36c-d4a6-43db-8fe9-74598da4c352"
                });

            //Add default Admin
            AppUserEntity admin = new()
            {
                Id = "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@local.com",
                NormalizedEmail = "ADMIN@LOCAL.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            admin.PasswordHash = hasher.HashPassword(admin, "123456@@");

            builder.Entity<AppUserEntity>()
                .HasData(admin);

            builder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>
                {

                    RoleId = "4e5e4a2b-9b92-40fa-87f2-1fefc574336b",
                    UserId = "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8"
                });
            #endregion
        }
    }
}
