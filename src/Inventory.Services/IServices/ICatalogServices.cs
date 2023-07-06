using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;


namespace Inventory.Services.IServices
{
    public interface ICatalogServices
    {
        Task<ResultResponse<IEnumerable<CatalogDTO>>> GetAll();
        Task<ResultResponse<CatalogDTO>> GetById(int id);
        Task<PaginationResponse<CatalogDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<CatalogDTO>> CreateCatalog(CatalogEditDTO dto);
        Task<ResultResponse<CatalogDTO>> UpdateCatalog(int id, CatalogEditDTO dto);
        Task<ResultResponse<CatalogDTO>> DeleteCatalog(int id);
    }
}
