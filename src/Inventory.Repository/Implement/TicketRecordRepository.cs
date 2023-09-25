using Inventory.Database.DbContext;
using Inventory.Model.Entity;

namespace Inventory.Repository.Implement
{
    public class TicketRecordRepository : BaseRepository<TicketRecord>, ITicketRecordRepository
    {
        public TicketRecordRepository(AppDbContext context) : base(context)
        {
        }
    }
}
