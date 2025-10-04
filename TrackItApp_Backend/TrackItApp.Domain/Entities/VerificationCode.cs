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
        ChangeEmail = 3,

        // Backup email
        ChangeBackupEmail = 4,
        RecoverWithBackupEmail = 5,
        ResetPasswordWithBackupEmail = 6
    }
    public class VerificationCode
    {
        public int VerificationCodeId { get; set; }
        public string Code { get; set; }
        public CodeType CodeType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Email { get; set; }
        public string DeviceId { get; set; }

        //Relations
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
