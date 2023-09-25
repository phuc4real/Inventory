using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service.Common.Request;
using Inventory.Service.Common.Response;

namespace Inventory.Service
{
    public interface ICatalogServices
    {
        Task<ResultResponse<IEnumerable<Catalog>>> GetList();
        Task<PaginationResponse<Catalog>> GetPagination(PaginationRequest request);
        Task<ResultResponse<Catalog>> GetById(int id);
        Task<ResultResponse<Catalog>> Create(UpdateCatalog dto);
        Task<ResultResponse> Update(int id, UpdateCatalog dto);
        Task<ResultResponse> Delete(int id);
        Task<ResultResponse> Any(int id);
    }
}
