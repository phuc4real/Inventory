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

        public async Task<IEnumerable<Export>> ExportByItem(Item item)
        {
            IQueryable<Export> query = _context.Exports;
            query = query.Where(x=>x.Items!.Contains(item))
                .Include(x => x.Details)!
                .ThenInclude(x => x.Item);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Export>> GetAllAsync(Expression<Func<Export, bool>>? filter = null)
        {
            IQueryable<Export> query = _context.Exports;
            query = query.Include(x => x.Details)!
                .ThenInclude(x => x.Item);

            if (filter != null) query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<Export> GetById(int id)
        {
            IQueryable<Export> query = _context.Exports;
            query = query.Where(x => x.Id == id)
                .Include(x=> x.Details)!
                .ThenInclude(x=>x.Item);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
