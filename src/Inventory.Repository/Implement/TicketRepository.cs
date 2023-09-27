using Inventory.Database.DbContext;
using Inventory.Model.Entity;

namespace Inventory.Repository.Implement
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context)
        {
        }
    }
}
