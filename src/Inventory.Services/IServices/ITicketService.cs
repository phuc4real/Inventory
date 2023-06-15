using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface ITicketService
    {
        Task<ResultResponse<IEnumerable<TicketDTO>>> GetAll();
        Task<ResultResponse<IEnumerable<TicketDTO>>> TicketsByItemId();
        Task<ResultResponse<TicketDTO>> GetTicketById(Guid id);
        Task<ResultResponse<TicketDTO>> CreateTicket(string token, TicketCreateDTO dto);
        Task<ResultResponse<TicketDTO>> UpdateTicketInfo(string token, TicketCreateDTO dto);
        Task<ResultResponse<TicketDTO>> PMApprove(string token, Guid ticketId);
        Task<ResultResponse<TicketDTO>> UpdateStatus(string token, Guid ticketId);
        Task<ResultResponse<TicketDTO>> CancelTicket(string token, Guid ticketId);
        Task<ResultResponse<TicketDTO>> CloseTicket(string token, TicketCancelDTO dto);
    }
}
