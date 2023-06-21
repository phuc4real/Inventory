using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Inventory.Core.Common;

namespace Inventory.Repository.DbContext
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Export> Exports { get; set; }
        public DbSet<ExportDetail> ExportDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole {
                    Id = "46a4f2b7-2a9e-4977-ae32-e0e5793e6267",
                    Name = InventoryRoles.Employee,
                    NormalizedName = InventoryRoles.Employee.ToUpper(),
                });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole { 
                    Id = "f8b59b69-fabb-4386-948e-5fb7054ffff4",
                    Name = InventoryRoles.PM,
                    NormalizedName = InventoryRoles.PM.ToUpper(),
                });
            builder.Entity<IdentityRole>()
                .HasData(new IdentityRole { 
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

            builder.Entity<Order>()
                .HasMany(e => e.Items)
                .WithMany(e => e.Orders)
                .UsingEntity<OrderDetail>(
                    l => l.HasOne<Item>(e=>e.Item).WithMany(e => e.OrderDetails),
                    r => r.HasOne<Order>().WithMany(e => e.Details)
                );

            builder.Entity<Export>()
                .HasMany(e => e.Items)
                .WithMany(e => e.Exports)
                .UsingEntity<ExportDetail>(
                    l => l.HasOne<Item>(e => e.Item).WithMany(e => e.ExportDetails),
                    r => r.HasOne<Export>(e => e.Export).WithMany(e => e.Details)
                );

            builder.Entity<Receipt>()
                .HasMany(e => e.Items)
                .WithMany(e => e.Receipts)
                .UsingEntity<ReceiptDetail>(
                    l => l.HasOne<Item>(e => e.Item).WithMany(e => e.ReceiptDetails),
                    r => r.HasOne<Receipt>().WithMany(e => e.Details)
                );

            builder.Entity<Ticket>()
                .HasMany(e => e.Items)
                .WithMany(e => e.Tickets)
                .UsingEntity<TicketDetail>(
                    l => l.HasOne<Item>(e => e.Item).WithMany(e => e.TicketDetails),
                    r => r.HasOne<Ticket>().WithMany(e => e.Details)
                );

            builder.Entity<Team>()
                .HasMany(e => e.Members)
                .WithOne(e => e.Team)
                .HasForeignKey(e => e.TeamId)
                .IsRequired(false);

            builder.Entity<Catalog>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<Item>()
                .HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
