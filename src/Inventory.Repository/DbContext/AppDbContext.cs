using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Inventory.Core.Common;
using Inventory.Repository.DataSeed;
using Microsoft.EntityFrameworkCore.Storage;

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

            builder.SeedingData();

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
