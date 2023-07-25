using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
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
