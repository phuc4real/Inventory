using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface IItemService
    {
        Task<ResultResponse<IEnumerable<ItemDTO>>> GetAll();
        Task<ResultResponse<ItemDTO>> GetById(Guid id);
        Task<ResultResponse<IEnumerable<ItemDTO>>> SearchByName(string name);
        Task<ResultResponse<ItemDTO>> CreateItem(ItemEditDTO dto);
        Task<ResultResponse<ItemDTO>> UpdateItem(Guid id,ItemEditDTO dto);
        Task<ResultResponse<ItemDTO>> DeleteItem(Guid id);

    }
}
