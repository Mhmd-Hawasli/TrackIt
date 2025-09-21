using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.Auth.AccountActivation;
using TrackItApp.Application.DTOs.UserDto.Auth.BackupEmail;
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
        Task<ApiResponse<object>> RequestChangeEmailAsync(RequestChangeEmailDto request, int userId, string deviceId);
        Task<ApiResponse<object>> VerifyChangeEmailAsync(VerifyChangeEmailDto request, int userId, string deviceId);

        //--------------------
        //Backup Email
        //--------------------
        Task<ApiResponse<object>> RequestAddBackupEmailAsync(RequestAddBackupEmailDto request, int userId, string deviceId);
        Task<ApiResponse<object>> VerifyAddBackupEmailAsync(VerifyAddBackupEmailDto request, int userId, string deviceId);
        Task<ApiResponse<object>> RemoveBackupEmailAsync(int userId);
        Task<ApiResponse<object>> RequestActivationWithBackupEmailAsync(RequestActivationWithBackupEmailDto request,string deviceId);
        Task<ApiResponse<LoginResponse>> VerifyActivationWithBackupEmailAsync(VerifyActivationWithBackupEmailDto request,string deviceId);
    }
}
