using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Common;
using TrackItApp.Domain.Repositories;
using TrackItApp.Infrastructure.Implementations.Persistence;

namespace TrackItApp.Infrastructure.Implementations.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        //Get Operation
        #region GetAllAsync (Dynamic)
        public virtual async Task<List<T>> GetAllAsync(QueryParameters filterModel, params string[] includes)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CountAsync
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
        #endregion

        #region GetByIdAsync
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        #endregion

        #region FindAsync
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        #endregion

        #region GetAllAsync
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        #endregion

        #region GetAllAsQueryable
        public virtual IQueryable<T> GetAllAsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        #endregion

        #region FirstOrDefaultAsync
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        #endregion

        #region FirstOrDefaultWithIncludesAsync
        public virtual async Task<T?> FirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        #endregion

        #region FirstOrDefaultWithoutSoftDeleteAsync
        public virtual async Task<T?> FirstOrDefaultWithoutSoftDeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        #endregion

        #region FirstOrDefaultWithIncludesWithoutSoftDeleteAsync
        public async Task<T?> FirstOrDefaultWithIncludesWithoutSoftDeleteAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            // Start the query by ignoring all global query filters, including the soft delete filter
            IQueryable<T> query = _dbSet.IgnoreQueryFilters();

            // Apply the includes for eager loading
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply the predicate and return the first or default item
            return await query.FirstOrDefaultAsync(predicate);
        }
        #endregion

        #region GetAllWithFilterAsync
        public virtual async Task<IEnumerable<T>> GetAllWithFilterAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply includes for eager loading
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply the filter predicate
            query = query.Where(predicate);

            return await query.ToListAsync();
        }
        #endregion

        //Add Operations
        #region AddAsync 
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        #endregion

        #region AddRangeAsync
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        #endregion

        //Update Operations
        #region Update
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        #endregion

        //Remove Operations
        #region Remove
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        #endregion

        #region RemoveRange
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        #endregion



    }
}
