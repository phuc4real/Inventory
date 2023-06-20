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
        Task<Export> GetById(int id);
        Task<IEnumerable<Export>> ExportByItem(Item item);
        Task<IEnumerable<Export>> GetAllAsync();
    }
}
