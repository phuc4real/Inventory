using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface ICatalogServices
    {
        Task<IEnumerable<Catalog>> GetAll();
    }
}
