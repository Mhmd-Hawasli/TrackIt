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

        //UserDictionary
        public IUserDictionaryRepository UserDictionaryRepository { get; private set; }
        public IWordRepository WordRepository { get; private set; }
        public IWordDetailRepository WordDetailRepository { get; private set; }
        public IWordConfidenceRepository WordConfidenceRepository { get; private set; }
        public IReviewHistoryRepository ReviewHistoryRepository { get; private set; }

        public UnitOfWork(
            AppDbContext context,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IVerificationCodeRepository verificationCodeRepository,
            IUserSessionRepository userSessionRepository,
            IUserDictionaryRepository userDictionaryRepository,
            IWordRepository wordRepository,
            IWordDetailRepository wordDetailRepository,
            IWordConfidenceRepository wordConfidenceRepository,
            IReviewHistoryRepository reviewHistoryRepository
            )
        {
            _context = context;
            UserRepository = userRepository;
            UserTypeRepository = userTypeRepository;
            VerificationCodeRepository = verificationCodeRepository;
            UserSessionRepository = userSessionRepository;
            UserDictionaryRepository = userDictionaryRepository;
            WordRepository = wordRepository;
            WordDetailRepository = wordDetailRepository;
            WordConfidenceRepository = wordConfidenceRepository;
            ReviewHistoryRepository = reviewHistoryRepository;
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
