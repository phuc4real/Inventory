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
        Task<ResultResponse<IEnumerable<ExportDTO>>> GetAll();
        Task<ResultResponse<IEnumerable<ExportDTO>>> GetExportByItemId(Guid id);
        Task<ResultResponse<ExportDTO>> GetById(int id);
        Task<ResultResponse<ExportDTO>> CreateExport(string token,ExportCreateDTO dto);
    }
}
