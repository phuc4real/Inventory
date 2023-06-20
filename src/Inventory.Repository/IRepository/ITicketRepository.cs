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
        Task<IEnumerable<Ticket>> GetTickets();
        Task<IEnumerable<Ticket>> GetTicketByTeam(Guid teamId);
        Task<IEnumerable<Ticket>> GetTicketByUser(string userid);
        Task<IEnumerable<Ticket>> GetTicketByItem(Item item);
        Task<IEnumerable<Ticket>> FindTickets(string filter);
        Task<Ticket> GetById(Guid id);

    }
}
