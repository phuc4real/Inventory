using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface ICatalogRepository : IRepository<CatalogEntity>
    {
        Task<PaginationList<CatalogEntity>> GetPagination(PaginationRequest request);
        Task<IEnumerable<CatalogEntity>> GetList(string name);
        Task<CatalogEntity> GetById(int id);
    }
}
