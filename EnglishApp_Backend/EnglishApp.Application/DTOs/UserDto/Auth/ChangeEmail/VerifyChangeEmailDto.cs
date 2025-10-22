using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.DTOs.UserDto.Auth.ChangeEmail
{
    public class VerifyChangeEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
