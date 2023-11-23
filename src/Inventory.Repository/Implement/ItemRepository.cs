using Inventory.Database.DbContext;
using Inventory.Model.Entity;

namespace Inventory.Repository.Implement
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
