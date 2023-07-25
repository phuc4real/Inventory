using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;

namespace Inventory.Repository.Repositories
{
    public class OrderInfoRepository : Repository<OrderInfoEntity>, IOrderInfoRepository
    {
        //private readonly AppDbContext _context;
        public OrderInfoRepository(AppDbContext context) : base(context)
        {
            //_context = context;
        }
    }
}
