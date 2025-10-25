using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Domain.Entities
{
    public class UserSession
    {
        [Key]
        public int UserSessionId { get; set; }
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        //Relations
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
