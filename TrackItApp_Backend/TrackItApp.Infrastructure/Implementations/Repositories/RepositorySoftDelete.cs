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

        //FirstOrDefaultAsync
        #region FirstOrDefaultAsync
        public override async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }
        #endregion
        #region FirstOrDefaultAsNoTrackingAsync
        public override async Task<T?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }
        #endregion


        //FirstOrDefaultWithIncludesAsync
        #region FirstOrDefaultWithIncludesAsync
        public override async Task<T?> FirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }
        #endregion  
        #region FirstOrDefaultWithIncludesAsNoTrackingAsync
        public override async Task<T?> FirstOrDefaultWithIncludesAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.AsNoTracking().Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }
        #endregion  



        //--------------------
        //Get All Operation
        //--------------------
        //WhereAsync
        #region WhereAsync
        public override async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
        }
        #endregion
        #region WhereAsNoTrackingAsync
        public override async Task<IEnumerable<T>> WhereAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
        }
        #endregion


        //GetAllWithFilterAsync 
        #region GetAllWithFilterAsync 
        public override async Task<List<T>> GetAllWithFilterAsync(Expression<Func<T, bool>> predicate, QueryParameters filterModel, params string[] includes)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region GetAllWithFilterAsNoTrackingAsync 
        public override async Task<List<T>> GetAllWithFilterAsNoTrackingAsync(Expression<Func<T, bool>> predicate, QueryParameters filterModel, params string[] includes)
        {
            throw new NotImplementedException();
        }
        #endregion  

        //GetAllAsync
        #region GetAllAsync
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }
        #endregion
        #region GetAllAsNoTrackingAsync
        public override async Task<IEnumerable<T>> GetAllAsNoTrackingAsync()
        {
            return await _dbSet.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
        }
        #endregion

        //GetAllAsQueryable
        #region GetAllAsQueryable
        public override IQueryable<T> GetAllAsQueryable()
        {
            return _dbSet.Where(e => !e.IsDeleted).AsQueryable();
        }
        #endregion
        #region GetAllAsNoTrackingAsQueryable
        public override IQueryable<T> GetAllAsNoTrackingAsQueryable()
        {
            return _dbSet.AsNoTracking().Where(e => !e.IsDeleted).AsQueryable();
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

        //RemoveRange
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
    }
}
