using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IExportDetailRepository : IRepository<ExportDetailEntity>
    {
        Task<PaginationList<ExportDetailEntity>> GetPagination(PaginationRequest request);
        Task<PaginationList<ExportDetailEntity>> GetPagination(PaginationRequest request, string userId);
        Task<PaginationList<ExportDetailEntity>> GetPagination(PaginationRequest request, Guid teamId);
    }
}
