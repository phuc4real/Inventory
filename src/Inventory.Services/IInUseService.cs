using Inventory.Core.Common;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service.Common;

namespace Inventory.Service
{
    public interface IInUseService
    {
        Task<PaginationResponse<InUse>> GetPagination(string token, PaginationRequest request);
    }
}
