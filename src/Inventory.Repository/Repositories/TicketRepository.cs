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
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            IQueryable<Ticket> query = _context.Tickets;
            query = query.Include(x => x.Details)!
                .ThenInclude(d => d.Item);

            return await query.ToListAsync();
        }

        public async Task<Ticket> GetById(Guid id)
        {
            IQueryable<Ticket> query = _context.Tickets;

            query = query.Where(x => x.Id == id)
                .Include(x => x.Details)!
                .ThenInclude(d => d.Item);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Ticket>> GetTicketOfUser(string userid)
        {
            IQueryable<Ticket> query = _context.Tickets;
            query = query.Where(x=> x.CreatedBy == userid)
                .Include(x => x.Details)!
                .ThenInclude(d => d.Item);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetWithFilter(string filter)
        {
            IQueryable<Ticket> query = _context.Tickets;
            query = query.Where(x => 
                x.Id.Equals(filter) ||
                x.Title!.Contains(filter) ||
                x.Description!.Contains(filter)
                );

            query = query.Include(x => x.Details)!
                .ThenInclude(d => d.Item);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> TicketsByItem(Item item)
        {
            IQueryable<Ticket> query = _context.Tickets;
            query = query.Where(x=> x.Items!.Contains(item))
                .Include(x => x.Details)!
                .ThenInclude(d => d.Item);

            return await query.ToListAsync();
        }
    }
}
