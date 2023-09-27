using Azure.Core;
using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Inventory.Repository.Implement
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Ctor & Field

        private readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        #endregion

        #region Default method

        public IQueryable<T> FindAll()
        {
            return _dbSet;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public async Task AddRangeAsync(List<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.IgnoreQueryFilters().AnyAsync(filter);
        }
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void RemoveRange(List<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void UpdateRage(List<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        #endregion

        #region Private

        private void CreateLog()
        {

        }

        private void UpdateLog()
        {

        }

        private void Deactive()
        {

        }

        #endregion
    }
}
