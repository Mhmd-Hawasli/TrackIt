using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Common;
using TrackItApp.Domain.Repositories;
using TrackItApp.Infrastructure.Implementations.Persistence;

namespace TrackItApp.Infrastructure.Implementations.Repositories
{
    public class RepositorySoftDelete<T> : RepositoryBase<T> where T : class, ISoftDelete
    {
        public RepositorySoftDelete(AppDbContext context) : base(context) { }




        //--------------------
        //Get One Operation
        //--------------------
        //GetByIdAsync
        #region GetByIdAsync
        public override async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return (entity != null && !entity.IsDeleted) ? entity : null;
        }
        #endregion


        //FirstOrDefaultAsync (with includes)
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
        #region FirstOrDefaultAsNoTrackingAsync
        public override async Task<T?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.AsNoTracking().Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }
        #endregion
        #region FirstOrDefaultWithSoftDeleteAsync
        public override async Task<T?> FirstOrDefaultWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return await base.FirstOrDefaultWithSoftDeleteAsync(predicate, includes);
        }
        #endregion 



        //--------------------
        //Get All Operation
        //--------------------
        //FindAsync
        #region FindAsync
        public override async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
        }
        #endregion
        #region FindAsNoTrackingAsync
        public override async Task<IEnumerable<T>> FindAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
        }
        #endregion
        #region FindWithSoftDeleteAsync
        public override async Task<IEnumerable<T>> FindWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            return await base.FindWithSoftDeleteAsync(predicate, includes);
        }
        #endregion


        //GetAllAsync
        #region GetAllAsync
        public override async Task<IEnumerable<T>> GetAllAsync(QueryParameters? filterModel = null, params string[] includes)
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
            return await query.Where(e => !e.IsDeleted).ToListAsync();
        }
        #endregion
        #region GetAllAsNoTrackingAsync
        public override async Task<IEnumerable<T>> GetAllAsNoTrackingAsync(QueryParameters? filterModel = null, params string[] includes)
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
            return await query.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
        }
        #endregion
        #region GetAllWithSoftDeleteAsync
        public override async Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(QueryParameters? filterModel = null, params string[] includes)
        {
            return await base.GetAllWithSoftDeleteAsync(filterModel, includes);
        }
        #endregion



        //--------------------
        //Check Operation
        //--------------------
        //CountAsync
        #region CountAsync
        public override async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).CountAsync(predicate);
        }
        #endregion   
        #region CountAsNoTrackingAsync
        public override async Task<int> CountAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(e => !e.IsDeleted).CountAsync(predicate);
        }
        #endregion
        #region CountWithSoftDeleteAsync
        public override async Task<int> CountWithSoftDeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return await base.CountWithSoftDeleteAsync(predicate);
        }
        #endregion


        //--------------------
        //Add Operations
        //--------------------
        //AddAsync
        #region AddAsync 
        public override async Task AddAsync(T entity)
        {
            await base.AddAsync(entity);
        }
        #endregion

        //AddRangeAsync
        #region AddRangeAsync
        public override async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await base.AddRangeAsync(entities);
        }
        #endregion



        //--------------------
        //Update Operations
        //--------------------
        //Update
        #region Update
        public override void Update(T entity)
        {
            base.Update(entity);
        }
        #endregion

        //UpdateRange
        #region UpdateRange
        public override void UpdateRange(IEnumerable<T> entities)
        {
            base.UpdateRange(entities);
        }
        #endregion



        //--------------------
        //Remove Operations
        //--------------------
        //Remove
        #region Remove
        public override void Remove(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
        #endregion
        #region Delete
        public override void Delete(T entity)
        {
            base.Delete(entity);
        }
        #endregion

        //RemoveRange
        #region RemoveRange
        public override void RemoveRange(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                entity.IsDeleted = true;
            }
            _dbSet.UpdateRange(entities);
        }
        #endregion
        #region DeleteRange
        public override void DeleteRange(IEnumerable<T> entities)
        {
            base.DeleteRange(entities);
        }
        #endregion

    }
}
