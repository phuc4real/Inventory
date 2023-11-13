using Inventory.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Inventory.Repository.Implement
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Ctor & Field

        private readonly DbSet<T> _dbSet;

        protected string? _userContext;

        public BaseRepository(AppDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        #endregion

        #region Default method

        public void SetUserContext(string userName)
        {
            _userContext = userName;
        }

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

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            BeforeAdd(entity, _userContext);
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(List<T> entities)
        {
            entities.ForEach(entity => BeforeAdd(entity, _userContext));
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.IgnoreQueryFilters().AnyAsync(filter);
        }

        public void Remove(T entity)
        {
            DeactiveEntity(entity, _userContext);
            //_dbSet.Remove(entity);
        }

        public void RemoveRange(List<T> entities)
        {
            entities.ForEach(entity => DeactiveEntity(entity, _userContext));
            //_dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            BeforeUpdate(entity, _userContext);
            _dbSet.Update(entity);
        }
        public void UpdateRage(List<T> entities)
        {
            entities.ForEach(entity => BeforeUpdate(entity, _userContext));
            _dbSet.UpdateRange(entities);
        }

        #endregion

        #region Private

        private void BeforeAdd(T entity, string? userContext)
        {
            var entityType = entity.GetType();
            entityType.GetProperty("CreatedAt")?.SetValue(entity, DateTime.UtcNow);
            entityType.GetProperty("CreatedBy")?.SetValue(entity, userContext);
            entityType.GetProperty("UpdatedAt")?.SetValue(entity, DateTime.UtcNow);
            entityType.GetProperty("UpdatedBy")?.SetValue(entity, userContext);
        }

        private void BeforeUpdate(T entity, string? userContext)
        {
            var entityType = entity.GetType();
            entityType.GetProperty("UpdatedAt")?.SetValue(entity, DateTime.UtcNow);
            entityType.GetProperty("UpdatedBy")?.SetValue(entity, userContext);
        }

        private void DeactiveEntity(T entity, string? userContext)
        {
            var entityType = entity.GetType();
            entityType.GetProperty("UpdatedAt")?.SetValue(entity, DateTime.UtcNow);
            entityType.GetProperty("UpdatedBy")?.SetValue(entity, userContext);
            entityType.GetProperty("IsInactive")?.SetValue(entity, true);
            entityType.GetProperty("InactiveAt")?.SetValue(entity, DateTime.UtcNow);
            entityType.GetProperty("InactiveBy")?.SetValue(entity, userContext);
        }

        #endregion
    }
}
