using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface IOrderService
    {
        Task<ResultResponse<IEnumerable<OrderDTO>>> GetList();
        Task<PaginationResponse<OrderDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<OrderDTO>> GetById(int id);
        Task<ResultResponse<OrderDTO>> CreateOrder(string token, OrderCreateDTO dto);
        Task<ResultResponse<OrderDTO>> UpdateOrderStatus(int id);
        Task<ResultResponse<OrderDTO>> CancelOrder(int id);
    }
}
