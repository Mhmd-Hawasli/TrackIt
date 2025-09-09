using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Domain.Entities
{
    public enum CodeType
    {
        // Account related
        ActivateAccount = 1,
        ResetPassword = 2,

        // Primary email
        RemoveEmail = 3,
        AddEmail = 4,

        // Backup email
        AddBackupEmail = 5,
        RemoveBackupEmail = 6,
        RecoverWithBackupEmail = 7
    }
    public class VerificationCode
    {
        public int VerificationCodeID { get; set; }
        public string Code { get; set; }
        public CodeType CodeType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string DeviceID { get; set; }

        //Relations
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
