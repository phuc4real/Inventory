using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface IExportDetailRepository : IRepository<ExportDetail>
    {
        Task<PaginationList<ExportDetail>> GetPagination(PaginationRequest request);
        Task<IEnumerable<ExportDetail>> GetList();
        Task<IEnumerable<ExportDetail>> GetList(Guid teamId);
        Task<IEnumerable<ExportDetail>> GetList(string userId);

    }
}
