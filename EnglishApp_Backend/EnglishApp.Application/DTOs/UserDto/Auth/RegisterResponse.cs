using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.Application.DTOs.UserDto.Auth
{
    public class RegisterResponse
    {
        public int UserId { get; set; }
        public string name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
