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
        Task<IEnumerable<ExportDetail>> GetAllAsync();
        //Search by Itemid, item name ,exportid ,username, userid, email
        Task<IEnumerable<ExportDetail>> SearchAsync(string filter);

    }
}
