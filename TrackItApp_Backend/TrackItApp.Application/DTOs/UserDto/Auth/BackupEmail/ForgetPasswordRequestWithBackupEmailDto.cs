using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.Auth.BackupEmail
{
    public class ForgetPasswordRequestWithBackupEmailDto
    {
        public string Input { get; set; }
        public string BackupEmail { get; set; }
    }
}
