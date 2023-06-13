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
        Task<ResultResponse<IEnumerable<ItemDetailDTO>>> GetAll();
        Task<ResultResponse<ItemDetailDTO>> GetById(Guid id);
        Task<ResultResponse<IEnumerable<ItemDetailDTO>>> SearchByName(string name);
        Task<ResultResponse<ItemDetailDTO>> CreateItem(string jwtToken, ItemEditDTO dto);
        Task<ResultResponse<ItemDetailDTO>> UpdateItem(string jwtToken, Guid id, ItemEditDTO dto);
        Task<ResultResponse<ItemDetailDTO>> DeleteItem(string jwtToken, Guid id);

    }
}
