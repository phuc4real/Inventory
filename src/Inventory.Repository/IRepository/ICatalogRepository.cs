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
    public interface ICatalogRepository : IRepository<Catalog>
    {
        Task<Catalog> FindById(int id);
        Task<IEnumerable<Catalog>> Search(string name);

        Task<PaginationList<Catalog>> GetPagination(PaginationRequest request);
    }
}
