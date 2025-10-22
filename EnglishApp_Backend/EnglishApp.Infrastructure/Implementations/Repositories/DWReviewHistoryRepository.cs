using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Interfaces.Repositories;
using EnglishApp.Domain.Entities;
using EnglishApp.Infrastructure.Implementations.Persistence;
using EnglishApp.Infrastructure.Implementations.Repositories;

namespace EnglishApp.Infrastructure.Implementations.Repositories
{
    public class DWReviewHistoryRepository : RepositoryBase<DWReviewHistory>, IDWReviewHistoryRepository
    {
        public DWReviewHistoryRepository(AppDbContext context) : base(context) { }

    }
}
