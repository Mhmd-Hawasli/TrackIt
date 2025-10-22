using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.UserDto.Auth;
using EnglishApp.Application.DTOs.UserDto.User;

namespace EnglishApp.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserGetByIdResponse>> GetUserInfoAsync(int userId);
        Task<ApiResponse<object>> UpdateUserAsync(UpdateUserRequest request, int userId);
        Task<ApiResponse<object>> DeactivateUserAsync(int userId);
    }
}
