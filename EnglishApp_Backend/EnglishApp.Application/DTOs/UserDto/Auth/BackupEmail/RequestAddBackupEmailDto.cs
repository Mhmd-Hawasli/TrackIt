using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.DTOs.UserDto.Auth.BackupEmail
{
    public class RequestAddBackupEmailDto
    {
        [Required]
        [EmailAddress]
        public string BackupEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
