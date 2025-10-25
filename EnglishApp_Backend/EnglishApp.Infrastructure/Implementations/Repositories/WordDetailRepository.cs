using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Interfaces.Repositories;
using EnglishApp.Domain.Entities;
using EnglishApp.Infrastructure.Implementations.Persistence;

namespace EnglishApp.Infrastructure.Implementations.Repositories
{
    public class WordDetailRepository : RepositoryBase<WordDetail>
        , IWordDetailRepository
    {
        public WordDetailRepository(AppDbContext context) : base(context) { }
    }
}
