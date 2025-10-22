using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.DTOs.UserDto.Auth.AccountActivation
{
    public class VerifyActivationCodeDto
    {
        [Required]
        public string Input{ get; set; }
        [Required]
        public string Code { get; set; }
    }
}
