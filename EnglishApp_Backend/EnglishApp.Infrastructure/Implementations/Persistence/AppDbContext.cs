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
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<DictionaryWord> DictionaryWords { get; set; }
        public DbSet<DWDetail> DWDetails { get; set; }
        public DbSet<DWConfidence> DWConfidences { get; set; }
        public DbSet<DWReviewHistory> DWReviewHistories { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Primary Keys
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UserSession>()
                .HasKey(us => us.UserSessionId);

            modelBuilder.Entity<UserType>()
                .HasKey(ut => ut.UserTypeId);

            modelBuilder.Entity<VerificationCode>()
                .HasKey(vc => vc.VerificationCodeId);

            modelBuilder.Entity<Dictionary>()
                .HasKey(d => d.DictionaryId);

            modelBuilder.Entity<DictionaryWord>()
                .HasKey(dw => dw.WordId);

            modelBuilder.Entity<DWDetail>()
                .HasKey(dwd => dwd.WordDetailId);

            modelBuilder.Entity<DWConfidence>()
                .HasKey(dwc => dwc.ConfidenceId);

            modelBuilder.Entity<DWReviewHistory>()
                .HasKey(drh => drh.ReviewId);
            #endregion

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
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserType)
                .WithMany(ut => ut.Users)
                .HasForeignKey(u => u.UserTypeId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserSessions)
                .WithOne(us => us.User)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.VerificationCodes)
                .WithOne(vc => vc.User)
                .HasForeignKey(vc => vc.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Dictionaries)
                .WithOne(d => d.CreatedByUser)
                .HasForeignKey(d => d.CreatedByUserId);

            modelBuilder.Entity<DictionaryWord>()
                .HasOne(dw => dw.Dictionary)
                .WithMany(d => d.DictionaryWords)
                .HasForeignKey(dw => dw.DictionaryId);

            modelBuilder.Entity<DictionaryWord>()
                .HasMany(dw => dw.DictionaryReviewHistories)
                .WithOne(drh => drh.DictionaryWord)
                .HasForeignKey(drh => drh.WordId);

            modelBuilder.Entity<DictionaryWord>()
                .HasMany(dw => dw.DictionaryWordDetails)
                .WithOne(dwd => dwd.DictionaryWord)
                .HasForeignKey(dwd => dwd.WordId);

            modelBuilder.Entity<DictionaryWord>()
                .HasOne(dw => dw.DictionaryWordConfidence)
                .WithMany(dwc => dwc.DictionaryWords)
                .HasForeignKey(dw => dw.ConfidenceId);

            modelBuilder.Entity<Dictionary>()
                .HasMany(d => d.DictionaryWordConfidences)
                .WithOne(dwc => dwc.Dictionary)
                .HasForeignKey(dwc => dwc.DictionaryId);
            #endregion
        }
    }
}
