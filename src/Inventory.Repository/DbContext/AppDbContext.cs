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

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { 
                    Name = InventoryRoles.Employee,
                    NormalizedName = InventoryRoles.Employee.ToUpper(),
                });
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { 
                    Name = InventoryRoles.PM,
                    NormalizedName = InventoryRoles.PM.ToUpper(),
                });
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { 
                    Name = InventoryRoles.DM,
                    NormalizedName = InventoryRoles.DM.ToUpper(),
                });
        }
    }
}
