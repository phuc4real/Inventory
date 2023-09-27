using Inventory.Core.Common;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Service.Common;

namespace Inventory.Service
{
    public interface IItemService
    {
        Task<ResultResponse<IEnumerable<Item>>> GetList(string? filter);
        Task<PaginationResponse<Item>> GetPagination(PaginationRequest request);
        Task<ResultResponse<ItemDetail>> GetById(Guid id);
        Task<ResultResponse<Item>> Create(string token, UpdateItem dto);
        Task<ResultResponse> Update(string token, Guid id, UpdateItem dto);
        Task<ResultResponse> Delete(string token, Guid id);
        Task<ResultResponse> Exists(List<Guid> ids);
        Task<ResultResponse> Order(List<OrderDetailEntity> details);
        Task<ResultResponse> Export(List<ExportDetailEntity> details);
    }
}
