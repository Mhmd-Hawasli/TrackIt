using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.User;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request);
    }
}
