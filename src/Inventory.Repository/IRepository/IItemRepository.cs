using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item> GetById(Guid id);
    }
}
