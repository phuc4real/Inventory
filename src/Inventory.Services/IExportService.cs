using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Export;

namespace Inventory.Service
{
    public interface IExportService
    {
        Task<ExportPaginationResponse> GetPaginationAsync(PaginationRequest request);
        Task<ExportObjectResponse> GetByIdAsync(int id);
        //Task<ExportObjectResponse> CreateFromTicketAsync(string adminId, string forUserId, TicketInfoEntity dto);
        Task<BaseResponse> UpdateExportStatusAsync(int id, BaseRequest request);
        Task<ExportChartDataResponse> GetChartDataAsync();
    }
}
