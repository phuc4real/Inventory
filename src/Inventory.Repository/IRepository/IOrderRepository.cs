using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IOrderRepository : IRepository<OrderEntity>
    {
        Task<PaginationList<OrderEntity>> GetPagination(PaginationRequest request);
        Task<IEnumerable<OrderEntity>> GetList();
        Task<OrderEntity> GetById(int id);
        Task<List<ResponseMessage>> GetCount();
    }
}
