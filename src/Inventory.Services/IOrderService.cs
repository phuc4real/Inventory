using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Order;

namespace Inventory.Service
{
    public interface IOrderService
    {
        Task<OrderPageResponse> GetPaginationAsync(PaginationRequest request);
        Task<OrderObjectResponse> GetByIdAsync(OrderRequest request);
        Task<OrderObjectResponse> CreateAsync(string token, UpdateOrderInfo dto);
        Task<ResultResponse> Decide(string token, int id, UpdateDecision decision);
        Task<ResultResponse> UpdateStatus(string token, int id);
        Task<ResultResponse> Cancel(string token, int id);
        Task<ResultResponse<IEnumerable<ResultMessage>>> GetCountByMonth();
    }
}
