using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.Auth.ChangeEmail
{
    public class ChangeEmailRequest
    {
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}
