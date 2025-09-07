using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;

namespace TrackItApp.Application.Interfaces.Repositories
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, string currentDeviceId);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string currentDeviceId);
        Task<ApiResponse<VerifyAccountResponse>> VerifyAccountCodeAsync(VerifyAccountRequest request, string currentDeviceId);
        Task<ApiResponse<object>> ResendCodeAsync(ResendCodeRequest request, string currentDeviceId);
    }
}
