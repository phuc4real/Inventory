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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Order> GetAllWithProperty => _context.Orders
            .IgnoreQueryFilters()
            .Include(x => x.OrderByUser)
            .Include(x => x.Details)!
            .ThenInclude(x => x.Item);

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Order> GetById(int id)
        {
            var query = GetAllWithProperty
                .Where(x=> x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Order>> OrdersByItem(Item item)
        {
            var query = GetAllWithProperty
                .Where(x => x.Items!.Contains(item));

            return await query.ToListAsync();
        }
    }
}
