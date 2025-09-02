using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Infrastructure.Implementations.Persistence;

namespace TrackItApp.Infrastructure.Implementations.Repositories
{
    internal class RepositoryAsNoTracking<T> : RepositoryBase<T> where T : class
    {
        public RepositoryAsNoTracking(AppDbContext context) : base(context) { }
    }
}
