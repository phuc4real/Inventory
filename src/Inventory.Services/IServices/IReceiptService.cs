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
    public interface IReceiptService
    {
        Task<ResultResponse<IEnumerable<ReceiptDTO>>> GetList();
        Task<PaginationResponse<ReceiptDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<ReceiptDTO>> ReceiptById(int id);
        Task<ResultResponse<ReceiptDTO>> CreateReceipt(string token, ReceiptCreateDTO dto);
    }
}
