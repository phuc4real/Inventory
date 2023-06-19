using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            IQueryable<Receipt> query = _context.Receipts;
            query = query.Include(x => x.Details)!
                .ThenInclude(d => d.Item);

            return await query.ToListAsync();
        }

        public async Task<Receipt> GetById(int id)
        {
            IQueryable<Receipt> query = _context.Receipts;
            query = query.Where(x => x.Id == id)
                .Include(x => x.Details)!
                .ThenInclude(x => x.Item);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Receipt>> ReceiptByItem(Item item)
        {
            IQueryable<Receipt> query = _context.Receipts;
            query = query.Where(x=>x.Items!.Contains(item))
                .Include(x => x.Details)!
                .ThenInclude(d => d.Item);

            return await query.ToListAsync();
        }
    }
}
