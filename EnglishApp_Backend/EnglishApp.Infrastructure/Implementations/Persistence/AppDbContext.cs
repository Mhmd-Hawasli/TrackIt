using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Infrastructure.Implementations.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }

        #region DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<UserDictionary> UserDictionaries { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordDetail> WordDetails { get; set; }
        public DbSet<WordConfidence> WordConfidences { get; set; }
        public DbSet<ReviewHistory> ReviewHistories { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Unique Constraints
            modelBuilder.Entity<VerificationCode>()
                .HasIndex(vc => new { vc.UserId, vc.DeviceId })
                .IsUnique();

            modelBuilder.Entity<UserSession>()
                .HasIndex(us => new { us.UserId, us.DeviceId })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            #endregion

            #region Relations
            #endregion
        }
    }
}
