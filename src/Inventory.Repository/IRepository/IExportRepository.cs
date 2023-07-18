using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface IExportRepository : IRepository<Export>
    {
        Task<PaginationList<Export>> GetPagination(PaginationRequest request);
        Task<IEnumerable<Export>> GetList();
        Task<Export> GetById(int id);
        Task<List<ResponseMessage>> GetCount();
    }
}
