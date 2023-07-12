using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface IExportService
    {
        Task<ResultResponse<IEnumerable<ExportDTO>>> GetList();
        Task<PaginationResponse<ExportDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<ExportDTO>> GetById(int id);
        Task<ResultResponse<ExportDTO>> CreateExport(string token, ExportCreateDTO dto);
        Task<ResultResponse<ExportDTO>> CancelExport(int id);
    }
}
