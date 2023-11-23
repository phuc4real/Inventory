using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using Inventory.Service.DTO.Order;

namespace Inventory.Service
{
    public interface IOrderService
    {
        Task<OrderPageResponse> GetPaginationAsync(PaginationRequest request);
        Task<OrderObjectResponse> GetByIdAsync(OrderRequest request);
        Task<OrderEntryListResponse> GetOrderEntries(OrderRequest request);
        Task<OrderObjectResponse> CreateOrUpdateAsync(OrderUpdateRequest request);
        Task<BaseResponse> UpdateOrderStatusAsync(OrderRequest request);
        Task<BaseResponse> CancelOrderAsync(OrderRequest request);
        Task<ChartDataResponse> GetOrderChartAsync();
        Task<BaseResponse> ApprovalOrderAsync(int recordId, CreateCommentRequest request);
    }
}
