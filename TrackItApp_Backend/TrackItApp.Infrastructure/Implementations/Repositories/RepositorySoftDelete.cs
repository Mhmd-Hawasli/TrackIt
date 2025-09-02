using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Repositories;
using TrackItApp.Infrastructure.Implementations.Persistence;

namespace TrackItApp.Infrastructure.Implementations.Repositories
{
    public class RepositorySoftDelete<T> : RepositoryBase<T> where T : class, ISoftDelete
    {
        public RepositorySoftDelete(AppDbContext context) : base(context) { }


        #region CountAsync
        public override async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).CountAsync(predicate);
        }
        #endregion
    }
}
