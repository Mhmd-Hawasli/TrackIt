using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.DTOs.UserDto.Auth
{
    public class ResendActivateCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
