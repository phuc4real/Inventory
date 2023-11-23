using Inventory.Database.DbContext;
using Inventory.Model.Entity;


namespace Inventory.Repository.Implement
{
    public class OrderEntryRepository : BaseRepository<OrderEntry>, IOrderEntryRepository
    {
        public OrderEntryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
