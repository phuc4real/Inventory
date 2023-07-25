using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;

namespace Inventory.Repository.Repositories
{
    public class TicketInfoRepository : Repository<TicketInfoEntity>, ITicketInfoRepository
    {
        //private readonly AppDbContext _context;

        public TicketInfoRepository(AppDbContext context) : base(context)
        {
            //_context = context;
        }
    }
}
