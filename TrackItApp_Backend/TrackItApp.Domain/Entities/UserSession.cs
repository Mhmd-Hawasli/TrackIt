using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public class UserSession
    {
        public int UserSessionID { get; set; }
        public string RefreshToken { get; set; }
        public string DeviceID { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        //Relations
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
