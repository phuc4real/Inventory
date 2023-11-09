using Inventory.Database.DbContext;
using Inventory.Model.Entity;

namespace Inventory.Repository.Implement
{
    internal class OrderRecordRepository : BaseRepository<OrderRecord>, IOrderRecordRepository
    {
        public OrderRecordRepository(AppDbContext context) : base(context)
        {
        }
    }
}
