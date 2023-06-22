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
        Task<ResultResponse<IEnumerable<ExportWithDetailDTO>>> GetAll();
        Task<ResultResponse<IEnumerable<ExportWithDetailDTO>>> GetExportByItemId(Guid id);
        Task<ResultResponse<ExportWithDetailDTO>> GetById(int id);
        Task<ResultResponse<ExportWithDetailDTO>> CreateExport(string token, ExportCreateDTO dto);
        Task<ResultResponse<ExportWithDetailDTO>> CancelExport(int id);
    }
}
