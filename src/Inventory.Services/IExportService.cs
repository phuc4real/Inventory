using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Export;

namespace Inventory.Service
{
    public interface IExportService
    {
        Task<ExportPaginationResponse> GetPaginationAsync(PaginationRequest request);
        Task<ExportObjectResponse> GetByIdAsync(ExportRequest request);
        Task<ExportEntryListResponse> GetEntriesAsync(ExportRequest request);
        Task<ExportObjectResponse> CreateFromTicketAsync(ExportCreateRequest request);
        Task<BaseResponse> UpdateExportStatusAsync(ExportRequest request);
        Task<ChartDataResponse> GetChartDataAsync();
    }
}
