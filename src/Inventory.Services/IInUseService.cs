using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service.Common.Request;
using Inventory.Service.Common.Response;

namespace Inventory.Service
{
    public interface IInUseService
    {
        Task<PaginationResponse<InUse>> GetPagination(string token, PaginationRequest request);
    }
}
