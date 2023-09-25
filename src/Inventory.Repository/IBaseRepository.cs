using Inventory.Core.Request;
using System.Linq.Expressions;

namespace Inventory.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>>? filter = null);
        //Task AddAsync(T entity);
        //Task AddRangeAsync(List<T> entities);
        //void Update(T entity);
        //void UpdateRage(List<T> entities);
        //void Remove(T entity);
        //void RemoveRange(List<T> entities);
        //Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
