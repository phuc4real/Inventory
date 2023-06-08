using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class CatalogService : ICatalogServices
    {
        private readonly ICatalogRepository _catalog;
        private readonly IUnitOfWork _unitOfWork;

        public CatalogService(ICatalogRepository catalog, IUnitOfWork unitOfWork)
        {
            _catalog = catalog;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Catalog>> GetAll()
        {
            return await _catalog.GetAsync();
        }
    }
}
