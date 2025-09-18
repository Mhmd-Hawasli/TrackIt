using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.Auth.AccountActivation;
using TrackItApp.Application.DTOs.UserDto.Auth.ChangeEmail;
using TrackItApp.Application.DTOs.UserDto.Auth.ChangePassword;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface IAuthService
    {
        //login related
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, string currentDeviceId);
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string currentDeviceId);
        Task<ApiResponse<object>> ResendActivationCodeAsync(ResendActivationCodeDto request, string currentDeviceId);
        Task<ApiResponse<LoginResponse>> VerifyActivationCodeAsync(VerifyActivationCodeDto request, string currentDeviceId);
        Task<ApiResponse<object>> LogoutAsync(int userId, string currentDeviceId);
        Task<ApiResponse<UpdateTokenResponse>> UpdateTokenAsync(UpdateTokenRequest request, string currentDeviceId);

        //--------------------
        //Change password
        //--------------------
        Task<ApiResponse<object>> ForgetPasswordRequestAsync(ForgetPasswordRequestDto request, string currentDeviceId);
        Task<ApiResponse<object>> ForgetPasswordVerifyCodeAsync(ForgetPasswordVerifyCodeDto request, string currentDeviceId);
        Task<ApiResponse<object>> ForgetPasswordResetPasswordAsync(ForgetPasswordResetPasswordDto request, string currentDeviceId);
        Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto request, int userId, string deviceId);

        //--------------------
        //Change Email
        //--------------------
        Task<ApiResponse<Object>> ChangeEmailRequestAsync(ChangeEmailRequest request, int userId, string deviceId);
        Task<ApiResponse<Object>> ChangeEmailVerifyAsync(ChangeEmailVerify request, int userId, string deviceId);
    }
}
