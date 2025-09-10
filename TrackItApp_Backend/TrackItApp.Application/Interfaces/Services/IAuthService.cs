using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface IAuthService
    {
        //login related
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, string currentDeviceId);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string currentDeviceId);
        Task<ApiResponse<object>> ResendCodeAsync(ResendCodeDto request, string currentDeviceId);
        Task<ApiResponse<LoginResponse>> VerifyAccountCodeAsync(VerifyAccountDto request, string currentDeviceId);
        Task<ApiResponse<object>> LogoutAsync(int userId, string currentDeviceId);
        Task<ApiResponse<UpdateTokenResponse>> UpdateTokenAsync(UpdateTokenRequest request, string currentDeviceId);
    }
}
