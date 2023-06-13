using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    Name = InventoryRoles.DM,
                    NormalizedName = InventoryRoles.DM.ToUpper(),
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
                    r => r.HasOne<Export>().WithMany(e => e.Details)
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
