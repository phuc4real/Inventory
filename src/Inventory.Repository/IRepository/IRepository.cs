using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
