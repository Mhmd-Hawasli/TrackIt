using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Repositories;

namespace TrackItApp.Domain.Entities
{
    public class User : ISoftDelete
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? BackupEmail { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        public bool IsDeleted { get; set; }

        //Relations
        public int UserTypeID { get; set; }
        public UserType UserType { get; set; }
        public ICollection<UserSession> UserSessions { get; set; }
        public ICollection<VerificationCode> VerificationCodes { get; set; }
        public ICollection<Dictionary> Dictionaries { get; set; }
    }
}
