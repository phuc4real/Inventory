using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service.Common.Request;
using Inventory.Service.Common.Response;

namespace Inventory.Service
{
    public interface ITicketService
    {
        Task<PaginationResponse<Ticket>> GetPagination(string token, PaginationRequest request);
        Task<ResultResponse<IEnumerable<Ticket>>> GetList();
        Task<ResultResponse<TicketWithHistory>> GetById(string token, int id);
        Task<ResultResponse<Ticket>> Create(string token, UpdateTicketInfo dto);
        Task<ResultResponse> Cancel(string token, int id);
        Task<ResultResponse> LeaderDecide(string token, int id, UpdateDecision decision);
        Task<ResultResponse> Decide(string token, int id, UpdateDecision decision);
        Task<ResultResponse> UpdateStatus(string token, int id);
        Task<ResultResponse<TicketCount>> GetTicketCount();
    }
}
