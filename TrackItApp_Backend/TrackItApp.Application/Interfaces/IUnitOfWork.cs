using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Interfaces.Repositories;
using TrackItApp.Domain.Repositories;

namespace TrackItApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserTypeRepository UserTypeRepository { get; }
        IVerificationCodeRepository VerificationCodeRepository { get; }
        IUserSessionRepository UserSessionRepository { get; }
        IDictionaryRepository DictionaryRepository { get; }
        IDictionaryWordRepository DictionaryWordRepository { get; }
        IDWDetailRepository DWDetailRepository { get; }
        IDWReviewHistoryRepository DWReviewHistoryRepository { get; }
        IDWConfidenceRepository DWConfidenceRepository { get; }

        //Method
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
