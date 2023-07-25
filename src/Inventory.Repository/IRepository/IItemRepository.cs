using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IItemRepository : IRepository<ItemEntity>
    {
        Task<PaginationList<ItemEntity>> GetPagination(PaginationRequest requestParams);
        Task<IEnumerable<ItemEntity>> GetList(string filter);
        Task<ItemEntity> GetById(Guid id);
        Task<IEnumerable<ItemEntity>> GetRange(List<Guid> ids);
    }
}
