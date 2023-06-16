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
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Item> GetById(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Items.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Item>> GetInUseItem()
        {
            IQueryable<Item> items = _context.Items;
            //IQueryable<int> exports = _context.Exports.Where(x=>x.IsCancel == false).Select(x=>x.Id);

            //items = items.Where(x => x.Exports!.Any(e => exports.Contains(e.Id)));

            items = items.Where(x => x.Exports!.Any(e => e.IsCancel == false));

            return await items.ToListAsync();
        }
    }
}
