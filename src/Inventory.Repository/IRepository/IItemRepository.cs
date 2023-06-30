using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item> GetById(Guid id);
        Task<PaginationList<Item>> GetListItem(ListItemRequest requestParams);
    }
}
