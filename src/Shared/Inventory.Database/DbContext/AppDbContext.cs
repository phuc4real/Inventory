using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Inventory.Database.DataSeed;
using Inventory.Model.Entity;

namespace Inventory.Database.DbContext
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        #region Ctor

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion

        #region DbSet

        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Export> Exports { get; set; }
        public DbSet<ExportEntry> ExportEntries { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderEntry> OrderEntries { get; set; }
        public DbSet<OrderRecord> OrderRecords { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketEntry> TicketEntries { get; set; }
        public DbSet<TicketRecord> TicketRecords { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.SeedingData();

            builder.Entity<Category>().HasKey(x => x.Id);
            builder.Entity<Comment>().HasKey(x => x.Id);
            builder.Entity<Export>().HasKey(x => x.Id);
            builder.Entity<ExportEntry>().HasKey(x => x.Id);
            builder.Entity<Item>().HasKey(x => x.Id);
            builder.Entity<Order>().HasKey(x => x.Id);
            builder.Entity<OrderEntry>().HasKey(x => x.Id);
            builder.Entity<OrderRecord>().HasKey(x => x.Id);
            builder.Entity<Status>().HasKey(x => x.Id);
            builder.Entity<Ticket>().HasKey(x => x.Id);
            builder.Entity<TicketEntry>().HasKey(x => x.Id);
            builder.Entity<TicketRecord>().HasKey(x => x.Id);
            builder.Entity<TicketType>().HasKey(x => x.Id);
        }
    }
}
