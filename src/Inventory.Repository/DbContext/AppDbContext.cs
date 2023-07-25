using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Inventory.Repository.DataSeed;

namespace Inventory.Repository.DbContext
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<CatalogEntity> Catalogs { get; set; }
        public DbSet<ExportEntity> Exports { get; set; }
        public DbSet<ExportDetailEntity> ExportDetails { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderInfoEntity> OrderInfos { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<TicketEntity> Tickets { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SeedingData();

            builder.Entity<OrderEntity>()
                .HasMany(o => o.History)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);

            builder.Entity<OrderInfoEntity>()
                .HasMany(e => e.Items)
                .WithMany(e => e.OrderInfo)
                .UsingEntity<OrderDetailEntity>(
                    l => l.HasOne<ItemEntity>(e => e.Item).WithMany(e => e.OrderDetails),
                    r => r.HasOne<OrderInfoEntity>().WithMany(e => e.Details)
                );

            builder.Entity<ExportEntity>()
                .HasMany(e => e.Items)
                .WithMany(e => e.Exports)
                .UsingEntity<ExportDetailEntity>(
                    l => l.HasOne<ItemEntity>(e => e.Item).WithMany(e => e.ExportDetails),
                    r => r.HasOne<ExportEntity>(e => e.Export).WithMany(e => e.Details)
                );

            builder.Entity<TicketEntity>()
                .HasMany(t => t.History)
                .WithOne(h => h.Ticket)
                .HasForeignKey(h => h.TicketId);

            builder.Entity<TicketInfoEntity>()
                .HasMany(x => x.Items)
                .WithMany(i => i.TicketInfo)
                .UsingEntity<TicketDetailEntity>(
                    l => l.HasOne<ItemEntity>(e => e.Item).WithMany(e => e.TicketDetails),
                    r => r.HasOne<TicketInfoEntity>().WithMany(e => e.Details));

            builder.Entity<TeamEntity>()
                .HasMany(e => e.Members)
                .WithOne(e => e.Team)
                .HasForeignKey(e => e.TeamId)
                .IsRequired(false);

            builder.Entity<DecisionEntity>()
                .HasKey(x => new { x.ById, x.Date });

            //builder.Entity<Catalog>()
            //    .HasQueryFilter(x => !x.IsDeleted);

            //builder.Entity<Item>()
            //    .HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
