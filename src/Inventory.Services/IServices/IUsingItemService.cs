using Inventory.Core.Common;
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
        Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetAllUsingItemAsync();
        Task<ResultResponse<IEnumerable<UsingItemDTO>>> SearchForUsingItemAsync(string filter);
    }
}
