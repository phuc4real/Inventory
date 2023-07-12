using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface ITicketService
    {
        Task<ResultResponse<IEnumerable<TicketDTO>>> GetList(string token);
        Task<PaginationResponse<TicketDTO>> GetPagination(string token, PaginationRequest request);
        Task<ResultResponse<IEnumerable<TicketDTO>>> GetMyTickets(string token);
        Task<ResultResponse<TicketDTO>> GetById(string token, Guid id);
        Task<ResultResponse<TicketDTO>> CreateTicket(string token, TicketCreateDTO dto);
        Task<ResultResponse<TicketDTO>> UpdateTicketInfo(string token, Guid ticketId, TicketCreateDTO dto);
        Task<ResultResponse<TicketDTO>> UpdatePMStatus(string token, Guid ticketId, string? rejectReason = null);
        Task<ResultResponse<TicketDTO>> UpdateStatus(string token, Guid ticketId);
        Task<ResultResponse<TicketDTO>> RejectTicket(string token, Guid ticketId, string rejectReason);
    }
}
