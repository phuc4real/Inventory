using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface ITicketRepository : IRepository<TicketEntity>
    {
        Task<PaginationList<TicketEntity>> GetPagination(PaginationRequest request);
        Task<PaginationList<TicketEntity>> GetPagination(PaginationRequest request, Guid teamId);
        Task<PaginationList<TicketEntity>> GetPagination(PaginationRequest request, string userId);
        Task<IEnumerable<TicketEntity>> GetList();
        Task<TicketEntity> GetById(int id);
        Task<TicketCount> GetCount();
    }
}
