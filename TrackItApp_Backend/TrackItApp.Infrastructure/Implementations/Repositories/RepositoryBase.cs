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


        //--------------------
        //Get One Operation
        //--------------------
        //GetByIdAsync
        #region GetByIdAsync
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        #endregion

        //FirstOrDefaultAsync
        #region FirstOrDefaultAsync
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        #endregion
        #region FirstOrDefaultAsNoTrackingAsync
        public virtual async Task<T?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }
        #endregion


        //FirstOrDefaultWithIncludesAsync
        #region FirstOrDefaultWithIncludesAsync
        public virtual async Task<T?> FirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
        #endregion  
        #region FirstOrDefaultWithIncludesAsNoTrackingAsync
        public virtual async Task<T?> FirstOrDefaultWithIncludesAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(predicate);
        }
        #endregion  



        //--------------------
        //Get All Operation
        //--------------------
        //WhereAsync
        #region WhereAsync
        public virtual async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        #endregion
        #region WhereAsNoTrackingAsync
        public virtual async Task<IEnumerable<T>> WhereAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
        #endregion


        //GetAllWithFilterAsync 
        #region GetAllWithFilterAsync 
        public virtual async Task<List<T>> GetAllWithFilterAsync(Expression<Func<T, bool>> predicate, QueryParameters filterModel, params string[] includes)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region GetAllWithFilterAsNoTrackingAsync 
        public virtual async Task<List<T>> GetAllWithFilterAsNoTrackingAsync(Expression<Func<T, bool>> predicate, QueryParameters filterModel, params string[] includes)
        {
            throw new NotImplementedException();
        }
        #endregion  

        //GetAllAsync
        #region GetAllAsync
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        #endregion
        #region GetAllAsNoTrackingAsync
        public virtual async Task<IEnumerable<T>> GetAllAsNoTrackingAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }
        #endregion

        //GetAllAsQueryable
        #region GetAllAsQueryable
        public virtual IQueryable<T> GetAllAsQueryable()
        {
            return _dbSet.AsQueryable();
        }
        #endregion
        #region GetAllAsNoTrackingAsQueryable
        public virtual IQueryable<T> GetAllAsNoTrackingAsQueryable()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
        #endregion



        //--------------------
        //Check Operation
        //--------------------
        //CountAsync
        #region CountAsync
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
        #endregion   
        #region CountAsNoTrackingAsync
        public virtual async Task<int> CountAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().CountAsync(predicate);
        }
        #endregion



        //--------------------
        //Add Operations
        //--------------------
        //AddAsync
        #region AddAsync 
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        #endregion

        //AddRangeAsync
        #region AddRangeAsync
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }
        #endregion



        //--------------------
        //Update Operations
        //--------------------
        //Update
        #region Update
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        #endregion



        //--------------------
        //Remove Operations
        //--------------------
        //Remove
        #region Remove
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        #endregion

        //RemoveRange
        #region RemoveRange
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        #endregion



    }
}
