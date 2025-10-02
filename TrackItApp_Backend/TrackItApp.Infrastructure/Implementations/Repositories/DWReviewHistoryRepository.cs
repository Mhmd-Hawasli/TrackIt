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
    public class DictionaryReviewHistoryRepository : RepositoryBase<DWReviewHistory>, IDWReviewHistoryRepository
    {
        public DictionaryReviewHistoryRepository(AppDbContext context) : base(context) { }

    }
}
