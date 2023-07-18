using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<PaginationList<Ticket>> GetPagination(PaginationRequest request);
        Task<IEnumerable<Ticket>> GetList();
        Task<IEnumerable<Ticket>> GetList(Guid teamId);
        Task<IEnumerable<Ticket>> GetList(string userid);
        Task<IEnumerable<Ticket>> GetList(Item item);
        Task<Ticket> GetById(Guid id);
        Task<TicketCountDTO> GetCount();
    }
}
