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

        public async Task<IEnumerable<Order>> GetAllAsync(Expression<Func<Order, bool>>? filter = null)
        {
            IQueryable<Order> query = _context.Orders.IgnoreQueryFilters();

            query = query.Include(x => x.Details)!
                .ThenInclude(x => x.Item);

            if (filter != null) query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<Order> GetById(int id)
        {
            IQueryable<Order> query = _context.Orders.IgnoreQueryFilters().Where(x=> x.Id == id);

            query = query.Include(x => x.Details)!
                .ThenInclude(x => x.Item);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Order>> OrdersByItem(Item item)
        {
            IQueryable<Order> query = _context.Orders.IgnoreQueryFilters();
            query = query.Where(x => x.Items!.Contains(item));

            return await query.ToListAsync();
        }
    }
}
