using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Interfaces.Repositories;
using TrackItApp.Domain.Entities;
using TrackItApp.Infrastructure.Implementations.Persistence;
using TrackItApp.Infrastructure.Implementations.Repositories;

namespace TrackItApp.Infrastructure.Implementations.Repositories
{
    public class DWReviewHistoryRepository : RepositoryBase<DWReviewHistory>, IDWReviewHistoryRepository
    {
        public DWReviewHistoryRepository(AppDbContext context) : base(context) { }

    }
}
