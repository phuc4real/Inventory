using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IExportRepository : IRepository<ExportEntity>
    {
        Task<PaginationList<ExportEntity>> GetPagination(PaginationRequest request);
        Task<IEnumerable<ExportEntity>> GetList();
        Task<ExportEntity> GetById(int id);
        Task<List<ResponseMessage>> GetCount();
    }
}
