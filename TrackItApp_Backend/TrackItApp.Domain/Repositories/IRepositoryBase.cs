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
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsNoTrackingAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<T?> FirstOrDefaultWithIncludesAsNoTrackingAsync(Expression<Func<T, bool>> predicate, params string[] includes);



        //--------------------
        // Get All Operation
        //--------------------
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> WhereAsNoTrackingAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllWithFilterAsync(Expression<Func<T, bool>> predicate, QueryParameters filterModel, params string[] includes);
        Task<List<T>> GetAllWithFilterAsNoTrackingAsync(Expression<Func<T, bool>> predicate, QueryParameters filterModel, params string[] includes);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsNoTrackingAsync();
        IQueryable<T> GetAllAsQueryable();
        IQueryable<T> GetAllAsNoTrackingAsQueryable();



        //--------------------
        // Check Operation
        //--------------------
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsNoTrackingAsync(Expression<Func<T, bool>> predicate);



        //--------------------
        // Add Operations
        //--------------------
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);



        //--------------------
        // Update Operations
        //--------------------
        void Update(T entity);



        //--------------------
        // Remove Operations
        //--------------------
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
