using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.Auth
{
    public class VerifyAccountRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
