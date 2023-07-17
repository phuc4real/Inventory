using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface ICatalogRepository : IRepository<Catalog>
    {
        Task<PaginationList<Catalog>> GetPagination(PaginationRequest request);
        Task<IEnumerable<Catalog>> GetList(string name);
        Task<Catalog> GetById(int id);
    }
}
