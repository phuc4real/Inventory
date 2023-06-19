using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface ITicketService
    {
        Task<ResultResponse<IEnumerable<TicketDTO>>> GetAll();
        Task<ResultResponse<IEnumerable<TicketDTO>>> TicketsByItemId(Guid itemId);
        Task<ResultResponse<TicketDTO>> GetTicketById(Guid id);
        Task<ResultResponse<TicketDTO>> CreateTicket(string token, TicketCreateDTO dto);
        Task<ResultResponse<TicketDTO>> UpdateTicketInfo(string token,Guid ticketId, TicketCreateDTO dto);
        Task<ResultResponse<TicketDTO>> PMStatus(string token, Guid ticketId, string? rejectReason = null);
        Task<ResultResponse<TicketDTO>> UpdateStatus(string token, Guid ticketId);
        Task<ResultResponse<TicketDTO>> RejectTicket(string token, Guid ticketId, string rejectReason);
        Task<ResultResponse<IEnumerable<TicketDTO>>> SearchTicket(string filter);
        Task<ResultResponse<IEnumerable<TicketDTO>>> ListTicketOfUser(string token);
    }
}
