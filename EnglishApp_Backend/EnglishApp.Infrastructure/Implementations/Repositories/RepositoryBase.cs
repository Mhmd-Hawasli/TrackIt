using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Common;
using EnglishApp.Domain.Repositories;
using EnglishApp.Infrastructure.Implementations.Persistence;

namespace EnglishApp.Infrastructure.Implementations.Repositories
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


        //FirstOrDefaultAsync (with includes)
        #region FirstOrDefaultAsync
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
        #endregion  
        #region FirstOrDefaultAsNoTrackingAsync
        public virtual async Task<T?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(predicate);
        }
        #endregion
        #region FirstOrDefaultWithSoftDeleteAsync
        public virtual async Task<T?> FirstOrDefaultWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
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
        //FindAsync
        #region FindAsync
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(predicate).ToListAsync();
        }
        #endregion
        #region FindAsNoTrackingAsync
        public virtual async Task<IEnumerable<T>> FindAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().Where(predicate).ToListAsync();
        }
        #endregion
        #region FindWithSoftDeleteAsync
        public virtual async Task<IEnumerable<T>> FindWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().Where(predicate).ToListAsync();
        }
        #endregion


        //GetAllAsync
        #region GetAllAsync
        public virtual async Task<IEnumerable<T>> GetAllAsync(QueryParameters? filterModel = null, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            //apply filter
            if (filterModel != null)
            {
                //filter logic
            }
            return await query.ToListAsync();
        }
        #endregion
        #region GetAllAsNoTrackingAsync
        public virtual async Task<IEnumerable<T>> GetAllAsNoTrackingAsync(QueryParameters? filterModel = null, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            //apply filter
            if (filterModel != null)
            {
                //filter logic
            }
            return await query.AsNoTracking().ToListAsync();
        }
        #endregion
        #region GetAllWithSoftDeleteAsync
        public virtual async Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(QueryParameters? filterModel = null, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            //apply filter
            if (filterModel != null)
            {
                //filter logic
            }
            return await query.ToListAsync();
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
        #region CountWithSoftDeleteAsync
        public virtual async Task<int> CountWithSoftDeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
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

        //UpdateRange
        #region UpdateRange
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
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
        #region Delete
        public virtual void Delete(T entity)
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
        #region DeleteRange
        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        #endregion


    }
}
