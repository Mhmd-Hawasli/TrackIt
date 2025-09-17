using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Common;

namespace TrackItApp.Domain.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {

        //--------------------
        // Get One Operation
        //--------------------
        Task<T?> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<T?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<T?> FirstOrDefaultWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes);


        //--------------------
        // Get All Operation
        //--------------------
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> FindAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> FindWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> GetAllAsync(QueryParameters? filterModel = null, params string[] includes);
        Task<IEnumerable<T>> GetAllAsNoTrackingAsync(QueryParameters? filterModel = null, params string[] includes);
        Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(QueryParameters? filterModel = null, params string[] includes);


        //--------------------
        // Check Operation
        //--------------------
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsNoTrackingAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountWithSoftDeleteAsync(Expression<Func<T, bool>> predicate);


        //--------------------
        // Add Operations
        //--------------------
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);


        //--------------------
        // Update Operations
        //--------------------
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        //--------------------
        // Remove Operations
        //--------------------
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
