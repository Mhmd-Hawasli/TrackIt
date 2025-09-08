using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.Auth
{
    public class RegisterResponse
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
