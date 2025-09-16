using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.User;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserGetByIdResponse>> GetUserInfoAsync(int userId);
        Task<ApiResponse<object>> UpdateUserAsync(UpdateUserRequest request, int userId);
        Task<ApiResponse<object>> DeactivateUserAsync(int userId);
    }
}
