using EnglishApp.Domain.Common;
using EnglishApp.Domain.Repositories;
using EnglishApp.Infrastructure.Implementations.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace EnglishApp.Infrastructure.Implementations.Repositories
{
    // The class now inherits from RepositoryBase and is constrained to entities that implement ISoftDelete
    public class RepositorySoftDelete<T> : RepositoryBase<T> where T : class, ISoftDelete
    {
        public RepositorySoftDelete(AppDbContext context) : base(context) { }


        // ==========================================================
        // Get All Operations  
        // ==========================================================

        #region GetQueryable
        public override IQueryable<T> GetQueryable()
        {
            return base.GetQueryable().Where(e => !e.IsDeleted);
        }
        #endregion

        #region GetAllWithFilterAsync
        public override async Task<(List<T>, int count)> GetAllWithFilterAndCountAsync(QueryParameters filterModel, params string[] includes)
        {
            IQueryable<T> query = GetQueryable();

            // Add includes to the query
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await ApplyFilterAsync(filterModel, query.Where(e => !e.IsDeleted));
        }
        public override async Task<List<T>> GetAllWithFilterAsync(QueryParameters filterModel, params string[] includes)
        {
            IQueryable<T> query = GetQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            (List<T> items, int _) = await ApplyFilterAsync(filterModel, query.Where(e => !e.IsDeleted));
            return items;
        }
        #endregion

        #region ApplyFilterAsync
        public override async Task<(List<T>, int count)> ApplyFilterAsync(QueryParameters filterModel, IQueryable<T> query)
        {
            return await base.ApplyFilterAsync(filterModel, query);
        }
        #endregion

        #region GetAllAsync
        public override async Task<IEnumerable<T>> GetAllAsync(params string[] includes)
        {
            IQueryable<T> query = GetQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(e => !e.IsDeleted).ToListAsync();
        }
        #endregion

        #region GetAllAsync -softDelete
        public override async Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(params string[] includes)
        {
            return await base.GetAllWithSoftDeleteAsync(includes);
        }
        #endregion

        #region GetAllAsync WithWhere
        public override async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = GetQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
        }
        #endregion

        #region GetAllAsync WithWhere -softDelete
        public override async Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return await base.GetAllWithSoftDeleteAsync(predicate, includes);
        }
        #endregion

        #region FindAllWithCancellationTokenAsync
        public override async Task<IEnumerable<T>> FindAllWithCancellationTokenAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync(cancellationToken);
        }
        #endregion

        // ==========================================================
        // Special Get Operations  
        // ==========================================================

        #region CountAsync
        public override async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).CountAsync(predicate);
        }
        #endregion

        #region AnyAsync
        public override async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).AnyAsync(predicate);
        }
        #endregion

        // ==========================================================
        // Get One Operations  
        // ==========================================================

        #region GetByIdAsync
        public override async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return (entity != null && !entity.IsDeleted) ? entity : null;
        }
        #endregion

        #region FirstOrDefaultAsync
        public override async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }
        #endregion

        #region FirstOrDefaultWithoutSoftDeleteAsync
        public override async Task<T?> FirstOrDefaultWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet.IgnoreQueryFilters();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }
        #endregion

        #region FirstOrDefaultWithSelectAsync
        public override async Task<TResult?> FirstOrDefaultWithSelectAsync<TResult>(Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selectExpression) where TResult : class
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .Select(selectExpression)
                .FirstOrDefaultAsync();
        }
        #endregion

        // ==========================================================
        // Add Operations
        // ==========================================================

        #region AddAsync
        public override async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        #endregion

        #region AddRangeAsync
        public override async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        #endregion

        // ==========================================================
        // Update Operations
        // ==========================================================

        #region Update
        public override void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        #endregion

        // ==========================================================
        // Delete Operations
        // ==========================================================

        #region Remove
        public override void Remove(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
        #endregion

        #region HardDelete
        public override void HardDelete(T entity)
        {
            base.HardDelete(entity);
        }
        #endregion

        #region RemoveRange
        public override void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
            }
            _dbSet.UpdateRange(entities);
        }
        #endregion

        #region RemoveAllAsync
        public override async Task RemoveAllAsync(Expression<Func<T, bool>> predicate)
        {
            var entitiesToSoftDelete = await _dbSet.Where(predicate).Where(e => !e.IsDeleted).ToListAsync();
            if (entitiesToSoftDelete.Any())
            {
                foreach (var entity in entitiesToSoftDelete)
                {
                    entity.IsDeleted = true;
                }
                _dbSet.UpdateRange(entitiesToSoftDelete);
            }
        }
        #endregion



    }
}