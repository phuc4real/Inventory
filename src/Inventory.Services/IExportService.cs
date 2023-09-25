using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Service.Common;
using Inventory.Service.Common.Request;
using Inventory.Service.Common.Response;

namespace Inventory.Service
{
    public interface IExportService
    {
        Task<ResultResponse<IEnumerable<Export>>> GetList();
        Task<PaginationResponse<Export>> GetPagination(PaginationRequest request);
        Task<ResultResponse<Export>> GetById(int id);
        Task<ResultResponse<Export>> CreateFromTicket(string adminId, string forUserId, TicketInfoEntity dto);
        Task<ResultResponse> UpdateStatus(string token, int id);
        Task<ResultResponse<IEnumerable<ResultMessage>>> GetCountByMonth();
    }
}
