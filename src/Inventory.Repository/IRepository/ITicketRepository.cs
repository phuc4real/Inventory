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
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<IEnumerable<Ticket>> GetWithFilter(string filter);
        Task<IEnumerable<Ticket>> TicketsByItem(Item item);
        Task<Ticket> GetById(Guid id);
        Task<IEnumerable<Ticket>> GetTicketOfUser(string userid);
    }
}
