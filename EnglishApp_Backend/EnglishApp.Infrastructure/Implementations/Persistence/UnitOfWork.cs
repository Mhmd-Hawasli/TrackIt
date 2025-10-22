using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Repositories;

namespace EnglishApp.Infrastructure.Implementations.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        //User
        public IUserTypeRepository UserTypeRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }
        public IVerificationCodeRepository VerificationCodeRepository { get; private set; }
        public IUserSessionRepository UserSessionRepository { get; private set; }

        //Dictionary
        public IDictionaryRepository DictionaryRepository { get; private set; }
        public IDictionaryWordRepository DictionaryWordRepository { get; private set; }
        public IDWDetailRepository DWDetailRepository { get; private set; }
        public IDWConfidenceRepository DWConfidenceRepository { get; private set; }
        public IDWReviewHistoryRepository DWReviewHistoryRepository { get; private set; }

        public UnitOfWork(
            AppDbContext context,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IVerificationCodeRepository verificationCodeRepository,
            IUserSessionRepository userSessionRepository,
            IDictionaryRepository dictionaryRepository,
            IDictionaryWordRepository dictionaryWordRepository,
            IDWDetailRepository dWDetailRepository,
            IDWConfidenceRepository dWConfidenceRepository,
            IDWReviewHistoryRepository dWReviewHistoryRepository
            )
        {
            _context = context;
            UserRepository = userRepository;
            UserTypeRepository = userTypeRepository;
            VerificationCodeRepository = verificationCodeRepository;
            UserSessionRepository = userSessionRepository;
            DictionaryRepository = dictionaryRepository;
            DictionaryWordRepository = dictionaryWordRepository;
            DWDetailRepository = dWDetailRepository;
            DWConfidenceRepository = dWConfidenceRepository;
            DWReviewHistoryRepository = dWReviewHistoryRepository;
        }

        //--------------------
        //Public Operations
        //--------------------
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }
        public async Task RollbackAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
