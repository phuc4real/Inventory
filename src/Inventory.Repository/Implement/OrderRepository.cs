using Inventory.Database.DbContext;
using Inventory.Model.Entity;

namespace Inventory.Repository.Implement
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
