using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Interfaces.Repositories;
using TrackItApp.Domain.Entities;
using TrackItApp.Infrastructure.Implementations.Persistence;

namespace TrackItApp.Infrastructure.Implementations.Repositories
{
    public class UserSessionRepository : RepositoryBase<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(AppDbContext context) : base(context) { }
    }
}
