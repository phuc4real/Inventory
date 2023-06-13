using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order> GetById(int id);
        Task<IEnumerable<Order>> GetAllAsync(Expression<Func<Order, bool>>? filter = null);
        Task<IEnumerable<Order>> OrdersByItem(Item item);
    }
}
