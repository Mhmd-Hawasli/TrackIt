using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Interfaces.Repositories;
using EnglishApp.Domain.Repositories;

namespace EnglishApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserTypeRepository UserTypeRepository { get; }
        IVerificationCodeRepository VerificationCodeRepository { get; }
        IUserSessionRepository UserSessionRepository { get; }
        IUserDictionaryRepository UserDictionaryRepository { get; }
        IWordRepository WordRepository { get; }
        IWordDetailRepository WordDetailRepository { get; }
        IReviewHistoryRepository ReviewHistoryRepository { get; }
        IWordConfidenceRepository WordConfidenceRepository { get; }

        //Method
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
