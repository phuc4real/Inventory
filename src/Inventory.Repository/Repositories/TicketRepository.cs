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

        private IQueryable<Ticket> GetAllWithProperty => _context.Tickets
            .Include(x => x.Details)!
            .ThenInclude(d => d.Item);

        public async Task<IEnumerable<Ticket>> GetTickets()
        {
            return await GetAllWithProperty.ToListAsync();
        }
        
        public async Task<IEnumerable<Ticket>> GetTicketByTeam(Guid teamId)
        {
            var query = GetAllWithProperty
                .Where(x=>x.CreatedByUser!.TeamId == teamId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketByUser(string userid)
        {
            var query = GetAllWithProperty
                .Where(x => x.CreatedBy == userid);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketByItem(Item item)
        {
            var query = GetAllWithProperty
                .Where(x => x.Items!.Contains(item));

            return await query.ToListAsync();
        }

        public async Task<Ticket> GetById(Guid id)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<Ticket>> FindTickets(string filter)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id.Equals(filter) ||
                            x.Title!.Contains(filter) ||
                            x.Description!.Contains(filter)
                );

            return await query.ToListAsync();
        }
    }
}
