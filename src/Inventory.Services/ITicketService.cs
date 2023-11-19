using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Order;
using Inventory.Service.DTO.Ticket;

namespace Inventory.Service
{
    public interface ITicketService
    {
        Task<TicketPageResponse> GetPaginationAsync(PaginationRequest request);
        Task<TicketObjectResponse> GetByIdAsync(TicketRequest request);
        Task<TicketEntryList> GetTicketEntries(TicketRequest request);
        Task<TicketTypeList> GetTicketType();
        Task<TicketObjectResponse> CreateOrUpdateAsync(TicketUpdateResquest request);
        Task<BaseResponse> CancelAsync(TicketRequest request);
        Task<BaseResponse> UpdateStatusAsync(TicketRequest request);
        Task<TicketSummaryObjectResponse> GetTicketSummary();
    }
}
