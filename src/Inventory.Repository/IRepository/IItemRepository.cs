using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> Search(string name);
        Task<Item> GetById(Guid id);
        Task<PaginationList<Item>> GetListItem(PaginationRequest requestParams);
    }
}
