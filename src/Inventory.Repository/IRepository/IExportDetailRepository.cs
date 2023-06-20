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
        Task<IEnumerable<ExportDetail>> GetUsingItem();
        Task<IEnumerable<ExportDetail>> GetUsingItemByTeam(Guid teamId);
        Task<IEnumerable<ExportDetail>> GetUsingItemByUser(string userId);
        Task<IEnumerable<ExportDetail>> SearchAsync(string filter);

    }
}
