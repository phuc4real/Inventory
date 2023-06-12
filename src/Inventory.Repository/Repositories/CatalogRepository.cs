using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class CatalogRepository : Repository<Catalog>, ICatalogRepository
    {
        private readonly AppDbContext _context;

        public CatalogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Catalog> FindById(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Catalogs.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
