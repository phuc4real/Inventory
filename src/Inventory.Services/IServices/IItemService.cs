using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface IItemService
    {
        Task<ResultResponse<IEnumerable<ItemDetailDTO>>> GetList(string? name);
        Task<PaginationResponse<ItemDetailDTO>> GetAll(PaginationRequest requestParams);
        Task<ResultResponse<ItemDetailDTO>> GetById(Guid id);
        Task<ResultResponse<ItemDetailDTO>> CreateItem(string token, ItemEditDTO dto);
        Task<ResultResponse<ItemDetailDTO>> UpdateItem(string token, Guid id, ItemEditDTO dto);
        Task<ResultResponse<ItemDetailDTO>> DeleteItem(string token, Guid id);
    }
}
