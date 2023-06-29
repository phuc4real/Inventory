using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface IItemService
    {
        Task<ResultResponse<IEnumerable<ItemDetailDTO>>> GetAll();
        Task<ResultResponse<ItemDetailDTO>> GetById(Guid id);
        Task<ResultResponse<IEnumerable<ItemDetailDTO>>> SearchByName(string name);
        Task<ResultResponse<ItemDetailDTO>> CreateItem(string token, ItemEditDTO dto);
        Task<ResultResponse<ItemDetailDTO>> UpdateItem(string token, Guid id, ItemEditDTO dto);
        Task<ResultResponse<ItemDetailDTO>> DeleteItem(string token, Guid id);
    }
}
