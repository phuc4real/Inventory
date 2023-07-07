using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetById(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<PaginationList<Order>> GetPagination(PaginationRequest request);
    }
}
