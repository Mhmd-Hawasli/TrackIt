using EnglishApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        // ==========================================================
        // Get All Operations  
        // ==========================================================
        IQueryable<T> GetQueryable();
        Task<(List<T>, int count)> GetAllWithFilterAndCountAsync(QueryParameters filterModel, params string[] includes);
        Task<List<T>> GetAllWithFilterAsync(QueryParameters filterModel, params string[] includes);
        Task<(List<T>, int count)> ApplyFilterAsync(QueryParameters filterModel, IQueryable<T> query);
        Task<IEnumerable<T>> GetAllAsync(params string[] includes);
        Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(params string[] includes);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> FindAllWithCancellationTokenAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

        // ==========================================================
        // Special Get Operations 
        // ==========================================================
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // ==========================================================
        // Get One Operations 
        // ==========================================================
        Task<T?> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<T?> FirstOrDefaultWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<TResult?> FirstOrDefaultWithSelectAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selectExpression) where TResult : class;

        // ==========================================================
        // Add Operations
        // ==========================================================
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // ==========================================================
        // Update Operations
        // ==========================================================
        void Update(T entity);

        // ==========================================================
        // Delete Operations
        // ==========================================================
        void Remove(T entity);
        void HardDelete(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task RemoveAllAsync(Expression<Func<T, bool>> predicate);
    }
}