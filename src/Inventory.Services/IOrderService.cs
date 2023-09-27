using Inventory.Core.Common;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service.Common;

namespace Inventory.Service
{
    public interface IOrderService
    {
        Task<ResultResponse<IEnumerable<Order>>> GetList();
        Task<PaginationResponse<Order>> GetPagination(PaginationRequest request);
        Task<ResultResponse<OrderWithHistory>> GetById(int id);
        Task<ResultResponse<Order>> Create(string token, UpdateOrderInfo dto);
        Task<ResultResponse> Decide(string token, int id, UpdateDecision decision);
        Task<ResultResponse> UpdateStatus(string token, int id);
        Task<ResultResponse> Cancel(string token, int id);
        Task<ResultResponse<IEnumerable<ResultMessage>>> GetCountByMonth();
    }
}
