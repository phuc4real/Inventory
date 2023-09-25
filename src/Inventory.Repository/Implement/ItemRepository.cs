using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Implement
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
