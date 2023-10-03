using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Export;

namespace Inventory.Service
{
    public interface IExportService
    {
        Task<ExportPaginationResponse> GetPaginationAsync(PaginationRequest request);
        Task<ExportObjectResponse> GetByIdAsync(ExportRequest request);
        //Task<ExportObjectResponse> CreateFromTicketAsync(string adminId, string forUserId, TicketInfoEntity dto);
        Task<BaseResponse> UpdateExportStatusAsync(ExportRequest request);
        Task<ExportChartDataResponse> GetChartDataAsync();
    }
}
