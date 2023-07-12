using Inventory.Core.Common;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface IUsingItemService
    {
        Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetList(string token);
        Task<PaginationResponse<UsingItemDTO>> GetPagination(string token, PaginationRequest request);
        Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetMyUsingItem(string token);
    }
}
