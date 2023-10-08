using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Item;

namespace Inventory.Service
{
    public interface IItemService
    {
        Task<ItemPaginationResponse> GetPaginationAsync(PaginationRequest request);
        Task<ItemObjectResponse> GetByIdAsync(ItemRequest request);
        Task<ItemObjectResponse> CreateAsync(ItemUpdateRequest request);
        Task<ItemObjectResponse> UpdateAsync(ItemUpdateRequest request);
        Task<BaseResponse> DeactiveAsync(ItemRequest request);
    }
}
