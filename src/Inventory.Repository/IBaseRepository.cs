using System.Linq.Expressions;

namespace Inventory.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        public IQueryable<T> FindAll();
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>>? filter = null);
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null);
        public Task AddAsync(T entity);
        public Task AddRangeAsync(List<T> entities);
        public void Update(T entity);
        public void UpdateRage(List<T> entities);
        public void Remove(T entity);
        public void RemoveRange(List<T> entities);
        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
    }
}
