using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Repositories;

namespace TrackItApp.Infrastructure.Implementations.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IUserTypeRepository UserTypeRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }
        public IVerificationCodeRepository VerificationCodeRepository { get; private set; }

        public UnitOfWork(
            AppDbContext context,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IVerificationCodeRepository verificationCodeRepository
            )
        {
            _context = context;
            UserRepository = userRepository;
            UserTypeRepository = userTypeRepository;
            VerificationCodeRepository = verificationCodeRepository;
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
