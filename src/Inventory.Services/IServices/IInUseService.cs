using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface IInUseService
    {
        Task<PaginationResponse<InUse>> GetPagination(string token, PaginationRequest request);
    }
}
