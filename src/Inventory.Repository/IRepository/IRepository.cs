using System.Linq.Expressions;

namespace Inventory.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void UpdateRage(List<T> entities);
        void Remove(T entity);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
