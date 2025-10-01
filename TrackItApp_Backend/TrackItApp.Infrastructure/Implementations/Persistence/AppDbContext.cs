using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Infrastructure.Implementations.Persistence
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
        public DbSet<DictionaryWordDetail> DictionaryWordDetails { get; set; }
        public DbSet<DictionaryWordConfidence> DictionaryWordConfidences { get; set; }
        public DbSet<DictionaryReviewHistory> DictionaryReviewHistories { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Primary Keys
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<UserSession>()
                .HasKey(us => us.UserSessionID);

            modelBuilder.Entity<UserType>()
                .HasKey(ut => ut.UserTypeID);

            modelBuilder.Entity<VerificationCode>()
                .HasKey(vc => vc.VerificationCodeID);

            modelBuilder.Entity<Dictionary>()
                .HasKey(d => d.DictionaryID);

            modelBuilder.Entity<DictionaryWord>()
                .HasKey(dw => dw.WordID);

            modelBuilder.Entity<DictionaryWordDetail>()
                .HasKey(dwd => dwd.WordDetailID);

            modelBuilder.Entity<DictionaryWordConfidence>()
                .HasKey(dwc => dwc.ConfidenceID);

            modelBuilder.Entity<DictionaryReviewHistory>()
                .HasKey(drh => drh.ReviewID);
            #endregion

            #region Unique Constraints
            modelBuilder.Entity<VerificationCode>()
                .HasIndex(vc => new { vc.UserID, vc.DeviceID })
                .IsUnique();

            modelBuilder.Entity<UserSession>()
                .HasIndex(us => new { us.UserID, us.DeviceID })
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
                .HasForeignKey(u => u.UserTypeID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserSessions)
                .WithOne(us => us.User)
                .HasForeignKey(u => u.UserID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.VerificationCodes)
                .WithOne(vc => vc.User)
                .HasForeignKey(vc => vc.UserID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Dictionaries)
                .WithOne(d => d.CreatedByUser)
                .HasForeignKey(d => d.CreatedByUserID);

            modelBuilder.Entity<DictionaryWord>()
                .HasOne(dw => dw.Dictionary)
                .WithMany(d => d.DictionaryWords)
                .HasForeignKey(dw => dw.DictionaryID);

            modelBuilder.Entity<DictionaryWord>()
                .HasMany(dw => dw.DictionaryReviewHistories)
                .WithOne(drh => drh.DictionaryWord)
                .HasForeignKey(drh => drh.WordID);

            modelBuilder.Entity<DictionaryWord>()
                .HasMany(dw => dw.DictionaryWordDetails)
                .WithOne(dwd => dwd.DictionaryWord)
                .HasForeignKey(dwd => dwd.WordID);

            modelBuilder.Entity<DictionaryWord>()
                .HasOne(dw => dw.DictionaryWordConfidence)
                .WithMany(dwc => dwc.dictionaryWords)
                .HasForeignKey(dw => dw.ConfidenceID);

            modelBuilder.Entity<Dictionary>()
                .HasMany(d => d.DictionaryWordConfidences)
                .WithOne(dwc => dwc.Dictionary)
                .HasForeignKey(dwc => dwc.DictionaryID);
            #endregion
        }
    }
}
