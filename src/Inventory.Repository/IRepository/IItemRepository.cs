using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<PaginationList<Item>> GetPagination(PaginationRequest requestParams);
        Task<IEnumerable<Item>> GetList(string filter);
        Task<Item> GetById(Guid id);
    }
}
