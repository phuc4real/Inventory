using Inventory.Core.Common;
using Inventory.Core.Const;
using Inventory.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Database.DataSeed
{
    public static class ModelBuilderExtension
    {
        public static void SeedingData(this ModelBuilder builder)
        {
            #region Seed Role

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

            #endregion

            #region Seed Default User

            var hasher = new PasswordHasher<AppUser>();
            //Add default SuperAdmin
            AppUser sa = new()
            {
                Id = "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                UserName = "sa",
                NormalizedUserName = "SA",
                Email = "sa@local.com",
                NormalizedEmail = "SA@LOCAL.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };
            sa.PasswordHash = hasher.HashPassword(sa, "123456@@");

            builder.Entity<AppUser>().HasData(sa);

            builder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>
                {
                    RoleId = "fc2a7273-a3c2-47be-bc55-aab11097e09a",
                    UserId = "d2f7a36c-d4a6-43db-8fe9-74598da4c352"
                });

            //Add default Admin
            AppUser admin = new()
            {
                Id = "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@local.com",
                NormalizedEmail = "ADMIN@LOCAL.COM",
                SecurityStamp = Guid.NewGuid().ToString()
            };

            admin.PasswordHash = hasher.HashPassword(admin, "123456@@");

            builder.Entity<AppUser>()
                .HasData(admin);

            builder.Entity<IdentityUserRole<string>>()
                .HasData(new IdentityUserRole<string>
                {

                    RoleId = "4e5e4a2b-9b92-40fa-87f2-1fefc574336b",
                    UserId = "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8"
                });

            #endregion

            #region Seed Status

            builder.Entity<Status>()
                .HasData(new Status { Id = 1, Name = StatusConstant.Pending, Description = "Pending" });
            builder.Entity<Status>()
                .HasData(new Status { Id = 2, Name = StatusConstant.Processing, Description = "Procesing" });
            builder.Entity<Status>()
                .HasData(new Status { Id = 3, Name = StatusConstant.Cancel, Description = "Cancel" });
            builder.Entity<Status>()
                .HasData(new Status { Id = 4, Name = StatusConstant.Rejected, Description = "Rejected" });
            builder.Entity<Status>()
                .HasData(new Status { Id = 5, Name = StatusConstant.Close, Description = "Close" });
            builder.Entity<Status>()
                .HasData(new Status { Id = 6, Name = StatusConstant.Done, Description = "Done" });

            #endregion
        }
    }
}
