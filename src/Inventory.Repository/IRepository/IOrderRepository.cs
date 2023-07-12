using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<PaginationList<Order>> GetPagination(PaginationRequest request);
        Task<IEnumerable<Order>> GetList();
        Task<Order> GetById(int id);

    }
}
