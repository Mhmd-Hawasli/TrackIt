using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackItApp.Application.DTOs.UserDto.User
{
    public class UsersWithSoftDeleteResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string IsDeleted { get; set; }
    }
}
