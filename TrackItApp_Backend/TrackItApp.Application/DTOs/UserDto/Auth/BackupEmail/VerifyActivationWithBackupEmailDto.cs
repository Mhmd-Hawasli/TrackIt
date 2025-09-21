using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.Auth.BackupEmail
{
    public class VerifyActivationWithBackupEmailDto
    {
        [Required]
        [EmailAddress]
        public string Input { get; set; }

        [Required]
        [EmailAddress]
        public string BackupEmail { get; set; }

        [Required]
        public string Code { get; set; }
    }
}

