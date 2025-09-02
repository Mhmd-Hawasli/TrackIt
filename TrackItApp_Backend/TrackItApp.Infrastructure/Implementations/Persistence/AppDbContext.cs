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
        public DbSet<UserType> userTypes { get; set; }
        public DbSet<VerificationCode> verificationCodes { get; set; }
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
            #endregion

            #region OneToMany Relations
            #endregion
        }
    }
}
