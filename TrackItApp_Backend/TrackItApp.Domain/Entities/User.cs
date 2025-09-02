using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }
        public string? BackupEmail { get; set; }
        public string PasswordHash { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

}
