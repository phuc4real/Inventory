using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class ExportRepository : Repository<Export>, IExportRepository
    {
        private readonly AppDbContext _context;

        public ExportRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Export> GetAllWithProperty => _context.Exports
                .Include(x => x.Details)!
                .ThenInclude(x => x.Item);

        public async Task<IEnumerable<Export>> ExportByItem(Item item)
        {
            var query = GetAllWithProperty
                .Where(x => x.Items!.Contains(item));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Export>> GetAllAsync()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Export> GetById(int id)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
