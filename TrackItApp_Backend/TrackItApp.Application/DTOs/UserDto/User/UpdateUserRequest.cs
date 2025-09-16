using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.User
{
    public class UpdateUserRequest
    {
        public string? Name { get; set; }
        public string? Username { get; set; }
    }
}
