using AutoMapper;

using Microsoft.AspNetCore.Localization;
using System.Net.Http.Headers;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.Auth.AccountActivation;
using TrackItApp.Application.DTOs.UserDto.Auth.BackupEmail;
using TrackItApp.Application.DTOs.UserDto.Auth.ChangeEmail;
using TrackItApp.Application.DTOs.UserDto.Auth.ChangePassword;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _tokenService = tokenService;
        }


        #region RegisterAsync
        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, string currentDeviceId)
        {
            //we used transaction to 
            //1- save user in database
            //2- send email and save codeModel in database
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                //get userType from database
                var existUserType = await _unitOfWork.UserTypeRepository.FirstOrDefaultAsync(ut => ut.UserTypeName == "user");
                if (existUserType == null)
                {
                    return new ApiResponse<RegisterResponse>("Role 'user' is not defined.");
                }

                //check if email is available
                var existEmail = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Email == request.Email.ToLower());
                if (existEmail != null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new ApiResponse<RegisterResponse>("The email address is already registered. Please use a different email address.");
                }

                //check if username is available
                var existUsername = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Username == request.Username);
                if (existUsername != null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new ApiResponse<RegisterResponse>("The username is already taken. Please choose a different username.");
                }

                //hash password before save it into database
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                //Map request to New UserModel
                var userModel = _mapper.Map<User>(request);
                userModel.UserType = existUserType;
                userModel.PasswordHash = hashedPassword;
                await _unitOfWork.UserRepository.AddAsync(userModel);
                await _unitOfWork.CompleteAsync();

                //send email verification
                await _emailService.SendEmailVerificationCode(userModel.UserID, userModel.Email, currentDeviceId, CodeType.ActivateAccount);

                //save new user in the database
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();

                //return response
                var response = _mapper.Map<RegisterResponse>(userModel);
                return new ApiResponse<RegisterResponse>(response, "User has been created successfully. A code has been sent to your email address.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse<RegisterResponse>(ex.Message);
            }
        }
        #endregion

        #region LoginAsync
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string currentDeviceId)
        {
            // get user info 
            var user = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(
                u => u.Username == request.Input || u.Email == request.Input.ToLower(),
                "UserType");
            if (user == null || user.IsDeleted == true || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponse<LoginResponse>("The login details are incorrect.");
            }

            //check if Is TwoFactor Enabled or user not verified
            if (user.IsTwoFactorEnabled == true || user.IsVerified == false)
            {
                await _emailService.SendEmailVerificationCode(user.UserID, user.Email, currentDeviceId, CodeType.ActivateAccount);
                await _unitOfWork.CompleteAsync();
                string message = user.IsVerified == false
                    ? "Your account has not been verified. Please check your email."
                    : "Two-factor authentication is enabled. Please check your email.";
                return new ApiResponse<LoginResponse>(true, null, message, null);
            }

            //check if user don't have active session
            var session = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsNoTrackingAsync(us => us.UserID == user.UserID && us.DeviceID == currentDeviceId);
            if (session == null || session.LastUpdatedAt < DateTime.UtcNow.AddMonths(-2))
            {
                if (session != null)
                {
                    _unitOfWork.UserSessionRepository.Remove(session);
                }
                await _emailService.SendEmailVerificationCode(user.UserID, user.Email, currentDeviceId, CodeType.ActivateAccount);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<LoginResponse>(true, null, "No active session found on this device. Please check your email.", null);
            }
            //generate accessToken and refreshToken
            var accessToken = _tokenService.CreateToken(user);
            (string refreshToken, string hashRefreshToken) = _tokenService.GenerateRefreshToken();

            //update userSession before login
            session.RefreshToken = hashRefreshToken;
            session.LastUpdatedAt = DateTime.UtcNow;
            session.IsRevoked = false;
            _unitOfWork.UserSessionRepository.Update(session);

            //save change to database
            await _unitOfWork.CompleteAsync();

            //return response
            var response = _mapper.Map<LoginResponse>(user);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            return new ApiResponse<LoginResponse>(response, "You have logged in successfully.");
        }
        #endregion

        #region ResendActivationCodeAsync
        public async Task<ApiResponse<object>> ResendActivationCodeAsync(ResendActivationCodeDto request, string currentDeviceId)
        {
            //normalize email
            request.Email = request.Email.ToLower();

            //get codeModel record from database via Email and DeviceID
            var verificationCode = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.User.Email == request.Email && vc.DeviceID == currentDeviceId && vc.CodeType == CodeType.ActivateAccount, "User");
            if (verificationCode == null)
            {
                return new ApiResponse<object>("You don’t have any expired code to resend.");
            }

            await _emailService.SendEmailVerificationCode(verificationCode.UserID, request.Email, currentDeviceId, verificationCode.CodeType);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "The code has been re-sent to your email.", null);

        }
        #endregion

        #region VerifyActivationCodeAsync
        public async Task<ApiResponse<LoginResponse>> VerifyActivationCodeAsync(VerifyActivationCodeDto request, string currentDeviceId)
        {
            //normalize email
            request.Email = request.Email.ToLower();


            //get verification code record from database
            var codeModel = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(
                vc => vc.DeviceID == currentDeviceId && vc.User.Email == request.Email,
                "User.UserSessions", "User.UserType");
            if (codeModel == null)
            {
                return new ApiResponse<LoginResponse>("There is no active code for this device.");
            }

            //check if codeModel is correct and it has the same type
            if (codeModel.CodeType != CodeType.ActivateAccount
                    || !BCrypt.Net.BCrypt.Verify(request.Code, codeModel.Code))
            {
                return new ApiResponse<LoginResponse>("The verification code you entered is invalid.");
            }

            //check if codeModel is expired and new email
            if (codeModel.ExpiresAt < DateTime.UtcNow)
            {
                await _emailService.SendEmailVerificationCode(codeModel.UserID, request.Email, currentDeviceId, CodeType.ActivateAccount);
                await _unitOfWork.CompleteAsync();
                return new ApiResponse<LoginResponse>("Your code has expired. Please check your email address.");
            }

            //remove the codeModel form database if everything is ok 
            _unitOfWork.VerificationCodeRepository.Remove(codeModel);

            //generate accessToken and refreshToken
            var accessToken = _tokenService.CreateToken(codeModel.User);
            (string refreshToken, string hashedRefreshToken) = _tokenService.GenerateRefreshToken();

            //save or update user session in database
            UserSession? userSession = codeModel.User.UserSessions.FirstOrDefault(us => us.DeviceID == currentDeviceId);
            if (userSession == null)
            {
                userSession = new UserSession()
                {
                    RefreshToken = hashedRefreshToken,
                    DeviceID = currentDeviceId,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    UserID = codeModel.UserID,
                    IsRevoked = false,
                };
                await _unitOfWork.UserSessionRepository.AddAsync(userSession);
            }
            else
            {
                userSession.RefreshToken = hashedRefreshToken;
                userSession.LastUpdatedAt = DateTime.UtcNow;
                userSession.IsRevoked = false;

                _unitOfWork.UserSessionRepository.Update(userSession);
            }

            //mark user as verified
            codeModel.User.IsVerified = true;

            //save change to database
            await _unitOfWork.CompleteAsync();

            //return response
            var response = _mapper.Map<LoginResponse>(codeModel.User);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            return new ApiResponse<LoginResponse>(response);
        }
        #endregion

        #region LogoutAsync
        public async Task<ApiResponse<object>> LogoutAsync(int userId, string currentDeviceId)
        {
            //get user model and his session
            var userSession = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsync(us => us.UserID == userId && us.DeviceID == currentDeviceId, "User");
            if (userSession == null)
            {
                return new ApiResponse<object>("UserSession Not Found.");
            }
            if (userSession.User == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }
            if (userSession.IsRevoked)
            {
                return new ApiResponse<object>("You are already logged out.");
            }
            //update userSession
            userSession.IsRevoked = true;
            _unitOfWork.UserSessionRepository.Update(userSession);

            await _unitOfWork.CompleteAsync();
            return new ApiResponse<object>(true, null, "You have been logged out successfully.", null);
        }
        #endregion

        #region UpdateTokenAsync
        public async Task<ApiResponse<UpdateTokenResponse>> UpdateTokenAsync(UpdateTokenRequest request, string currentDeviceId)
        {
            //check token's secret key and return userId
            int userId = _tokenService.ValidateExpiredAccessToken(request.AccessToken);

            //check userSession in database by UserId and DeviceId
            var userSession = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsync(us => us.UserID == userId && us.DeviceID == currentDeviceId, "User.UserType");
            if (userSession == null)
            {
                return new ApiResponse<UpdateTokenResponse>("No active session found on this device. Please log in again.");
            }

            //check if userSession is valid
            if (userSession.IsRevoked == true || userSession.LastUpdatedAt < DateTime.UtcNow.AddMonths(-2))
            {
                return new ApiResponse<UpdateTokenResponse>("Your session is invalid. Please log in again.");
            }

            //check if refresh token is correct
            if (!BCrypt.Net.BCrypt.Verify(request.RefreshToken, userSession.RefreshToken))
            {
                return new ApiResponse<UpdateTokenResponse>("Your refresh token is no longer valid.");
            }

            //generate new refresh token and access token

            string accessToken = _tokenService.CreateToken(userSession.User);
            (string refreshToken, string hashRefreshToken) = _tokenService.GenerateRefreshToken();

            //update userSession
            userSession.RefreshToken = hashRefreshToken;
            userSession.LastUpdatedAt = DateTime.UtcNow;
            _unitOfWork.UserSessionRepository.Update(userSession);

            //save changes to database
            await _unitOfWork.CompleteAsync();

            //return response dto
            var response = new UpdateTokenResponse
            {
                NewAccessToken = accessToken,
                NewRefreshToken = refreshToken,
            };
            return new ApiResponse<UpdateTokenResponse>(response);
        }
        #endregion

        //--------------------
        //Change password
        //--------------------

        #region ForgetPasswordRequestAsync
        public async Task<ApiResponse<object>> ForgetPasswordRequestAsync(ForgetPasswordRequestDto request, string currentDeviceId)
        {
            //get user info
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), "UserType");
            if (user == null)
            {
                return new ApiResponse<object>("User not found.");
            }
            await _emailService.SendEmailVerificationCode(user.UserID, user.Email, currentDeviceId, CodeType.ResetPassword);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "An email has been sent to your address. Please check your inbox.", null);
        }
        #endregion

        #region ForgetPasswordVerifyCodeAsync
        public async Task<ApiResponse<object>> ForgetPasswordVerifyCodeAsync(ForgetPasswordVerifyCodeDto request, string currentDeviceId)
        {
            //get codeModel info and user info
            var codeModel = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.User.Email == request.Email.ToLower() && vc.DeviceID == currentDeviceId, "User");
            if (codeModel == null)
            {
                return new ApiResponse<object>("Verification record not found. Please submit the request again.");
            }
            if (codeModel.User == null)
            {
                return new ApiResponse<object>("User not found.");
            }
            if (codeModel.ExpiresAt < DateTime.UtcNow
                    || codeModel.CodeType != CodeType.ResetPassword
                    || !BCrypt.Net.BCrypt.Verify(request.Code, codeModel.Code))
            {
                return new ApiResponse<object>("Your Verification code is not valid");
            }
            //We haven’t deleted the verification code because it’s needed for the next step.

            return new ApiResponse<object>(true, null, "Email verification successful. Please proceed to the next step.", null);
        }
        #endregion

        #region ForgetPasswordResetPasswordAsync
        public async Task<ApiResponse<object>> ForgetPasswordResetPasswordAsync(ForgetPasswordResetPasswordDto request, string currentDeviceId)
        {
            //get codeModel info and user info
            var codeModel = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.User.Email == request.Email.ToLower() && vc.DeviceID == currentDeviceId, "User.UserSessions");
            if (codeModel == null)
            {
                return new ApiResponse<object>("Verification record not found. Please submit the request again.");
            }
            if (codeModel.User == null)
            {
                return new ApiResponse<object>("User not found.");
            }
            if (codeModel.ExpiresAt < DateTime.UtcNow
                    || codeModel.CodeType != CodeType.ResetPassword
                    || !BCrypt.Net.BCrypt.Verify(request.Code, codeModel.Code))
            {
                return new ApiResponse<object>("Your Verification code is not valid");
            }
            //check if request newPassword is null or empty
            if (string.IsNullOrEmpty(request.NewPassword))
            {
                return new ApiResponse<object>("The new password cannot be empty.");
            }

            //check if new password is the same of the old password
            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, codeModel.User.PasswordHash))
            {
                return new ApiResponse<object>("New password must be different from the old password.");
            }

            //update user password
            codeModel.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            codeModel.User.IsVerified = true;
            _unitOfWork.UserRepository.Update(codeModel.User);

            //remove session form other device
            var sessions = codeModel.User.UserSessions.Where(us => us.DeviceID != currentDeviceId).ToList();
            _unitOfWork.UserSessionRepository.RemoveRange(sessions);

            //remove verification code record
            _unitOfWork.VerificationCodeRepository.Remove(codeModel);

            //save change to database 
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "Your password has been successfully reset.", null);
        }
        #endregion

        #region ChangePasswordAsync
        public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto request, int userId, string deviceId)
        {
            //get user info from database
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserID == userId, "UserSessions");
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            //check if old password is correct
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return new ApiResponse<object>("Your old password is incorrect.");
            }

            //check if new password is null or empty and send error response
            if (string.IsNullOrEmpty(request.NewPassword))
            {
                return new ApiResponse<object>("The new password cannot be null or empty.");
            }

            if (request.OldPassword == request.NewPassword)
            {
                return new ApiResponse<object>("The new password cannot be the same as the old password.");
            }

            //hash new password and add it to database
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordHash = hashPassword;
            _unitOfWork.UserRepository.Update(user);

            var sessionListToRemove = user.UserSessions.Where(s => s.DeviceID != deviceId).ToList();
            _unitOfWork.UserSessionRepository.RemoveRange(sessionListToRemove);

            await _unitOfWork.CompleteAsync();
            return new ApiResponse<object>(true, null, "Your password has been changed successfully.", null);

        }

        #endregion


        //--------------------
        //Change password
        //--------------------

        #region RequestChangeEmailAsync
        public async Task<ApiResponse<object>> RequestChangeEmailAsync(RequestChangeEmailDto request, int userId, string deviceId)
        {
            // Normalize new email
            request.NewEmail = request.NewEmail.ToLower();

            // Get user info from database
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserID == userId);

            // Check if user exists
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            // Verify the user's current password for security
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponse<object>("The current password you entered is incorrect.");
            }

            // Check if the new email is the same as the current email
            if (user.Email == request.NewEmail)
            {
                return new ApiResponse<object>("Your new email is the same as your current email. Please choose a different one.");
            }

            // Check if the new email is already used by another user (including soft-deleted users)
            var existEmail = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Email == request.NewEmail);
            if (existEmail != null)
            {
                return new ApiResponse<object>("This email is already used by someone else. Please pick another email address.");
            }

            // Check if the new email is the same as the user's backup email
            if (user.BackupEmail == request.NewEmail)
            {
                return new ApiResponse<object>("You cannot use your backup email as the new email. Please pick another email address.");
            }

            // Send verification code to the new email address
            await _emailService.SendEmailVerificationCode(userId, request.NewEmail, deviceId, CodeType.ChangeEmail);
            await _unitOfWork.CompleteAsync();

            // Return success response
            return new ApiResponse<object>(true, null, "An email has been sent to your new email address. Please check your email.", null);
        }
        #endregion

        #region VerifyChangeEmailAsync
        public async Task<ApiResponse<object>> VerifyChangeEmailAsync(VerifyChangeEmailDto request, int userId, string deviceId)
        {
            // Normalize new email
            request.NewEmail = request.NewEmail.ToLower();

            // Get Verification Code base on userId and DeviceId
            var codeModel = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.DeviceID == deviceId && vc.UserID == userId, "User.UserSessions");
            if (codeModel == null
                || codeModel.CodeType != CodeType.ChangeEmail
                || codeModel.ExpiresAt < DateTime.UtcNow
                || codeModel.Email != request.NewEmail
                || !BCrypt.Net.BCrypt.Verify(request.Code, codeModel.Code))
            {
                return new ApiResponse<object>("verification code is not valid.");
            }

            //update user email
            codeModel.User.Email = request.NewEmail;
            _unitOfWork.UserRepository.Update(codeModel.User);

            //remove session from other device
            var sessionListToRemove = codeModel.User.UserSessions.Where(us => us.DeviceID != deviceId).ToList();
            _unitOfWork.UserSessionRepository.RemoveRange(sessionListToRemove);

            //remove verification code form database 
            _unitOfWork.VerificationCodeRepository.Remove(codeModel);

            await _unitOfWork.CompleteAsync();

            // Return success response
            return new ApiResponse<object>(true, null, "Your email has been changed successfully.", null);
        }
        #endregion

        //--------------------
        //Backup Email
        //--------------------

        #region AddBackupEmailRequestAsync
        public async Task<ApiResponse<object>> RequestAddBackupEmailAsync(RequestAddBackupEmailDto request, int userId, string deviceId)
        {
            //Normalize
            request.BackupEmail = request.BackupEmail.ToLower();
            //get user info
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            if (user.BackupEmail != null)
            {
                return new ApiResponse<object>("The user already has a backup email. Please remove it first.");
            }

            if (user.Email == request.BackupEmail)
            {
                return new ApiResponse<object>("The backup email cannot be the same as the primary email.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponse<object>("The current password you entered is incorrect.");
            }

            //send verification code to new backup email
            await _emailService.SendEmailVerificationCode(userId, request.BackupEmail, deviceId, CodeType.ChangeBackupEmail);
            await _unitOfWork.CompleteAsync();

            //return message response
            return new ApiResponse<object>(true, null, "A verification email has been sent to your backup email. Please check your inbox.", null);
        }
        #endregion

        #region VerifyAddBackupEmailAsync
        public async Task<ApiResponse<object>> VerifyAddBackupEmailAsync(VerifyAddBackupEmailDto request, int userId, string deviceId)
        {
            request.BackupEmail = request.BackupEmail.ToLower();

            var codeModel = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.UserID == userId && vc.DeviceID == deviceId, "User");
            if (codeModel == null
                || codeModel.CodeType != CodeType.ChangeBackupEmail
                || codeModel.ExpiresAt < DateTime.UtcNow
                || codeModel.Email != request.BackupEmail
                || !BCrypt.Net.BCrypt.Verify(request.Code, codeModel.Code))
            {
                return new ApiResponse<object>("verification code is not valid.");
            }

            if (codeModel.User.BackupEmail != null)
            {
                return new ApiResponse<object>("The user already has a backup email. Please remove it first.");
            }

            //update user backup email
            codeModel.User.BackupEmail = request.BackupEmail;
            _unitOfWork.UserRepository.Update(codeModel.User);

            //remove verification code form database
            _unitOfWork.VerificationCodeRepository.Remove(codeModel);
            await _unitOfWork.CompleteAsync();

            //return message response
            return new ApiResponse<object>(true, null, "The new backup email has been saved successfully.", null);
        }
        #endregion

        #region RemoveBackupEmailAsync
        public async Task<ApiResponse<object>> RemoveBackupEmailAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            if (user.BackupEmail == null)
            {
                return new ApiResponse<object>("There is no backup email to remove.");
            }

            //update user info
            user.BackupEmail = null;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CompleteAsync();

            //return message response
            return new ApiResponse<object>(true, null, "The backup email has been removed successfully.", null);
        }
        #endregion

        #region RequestActivationWithBackupEmailAsync
        public async Task<ApiResponse<object>> RequestActivationWithBackupEmailAsync(RequestActivationWithBackupEmailDto request, string deviceId)
        {
            //normalize email
            request.BackupEmail = request.BackupEmail.ToLower();

            //get user info
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.Email == request.Input.ToLower() || u.Username == request.Input);
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            if (user.BackupEmail != request.BackupEmail
                || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponse<object>("Invalid information.");
            }

            //send verfiacation code to backup email
            await _emailService.SendEmailVerificationCode(user.UserID, request.BackupEmail, deviceId, CodeType.RecoverWithBackupEmail);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "An email has been sent to your backup email. Please check your inbox.", null);
        }
        #endregion

        #region VerifyActivationWithBackupEmailAsync
        public async Task<ApiResponse<LoginResponse>> VerifyActivationWithBackupEmailAsync(VerifyActivationWithBackupEmailDto request, string deviceId)
        {
            //normalize email
            request.BackupEmail = request.BackupEmail.ToLower();

            //get verification code record from database
            var codeModel = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(
                vc => vc.DeviceID == deviceId && (vc.User.Email == request.Input.ToLower() || vc.User.Username == request.Input),
                "User.UserSessions", "User.UserType");
            if (codeModel == null)
            {
                return new ApiResponse<LoginResponse>("There is no active code for this device.");
            }

            //check if codeModel is correct and it has the same type
            if (codeModel.CodeType != CodeType.RecoverWithBackupEmail
                    || !BCrypt.Net.BCrypt.Verify(request.Code, codeModel.Code)
                    || codeModel.ExpiresAt < DateTime.UtcNow)
            {
                return new ApiResponse<LoginResponse>("The verification code you entered is invalid.");
            }

            //remove the codeModel form database if everything is ok 
            _unitOfWork.VerificationCodeRepository.Remove(codeModel);

            //generate accessToken and refreshToken
            var accessToken = _tokenService.CreateToken(codeModel.User);
            (string refreshToken, string hashedRefreshToken) = _tokenService.GenerateRefreshToken();

            //save or update user session in database
            UserSession? userSession = codeModel.User.UserSessions.FirstOrDefault(us => us.DeviceID == deviceId);
            if (userSession == null)
            {
                userSession = new UserSession()
                {
                    RefreshToken = hashedRefreshToken,
                    DeviceID = deviceId,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    UserID = codeModel.UserID,
                    IsRevoked = false,
                };
                await _unitOfWork.UserSessionRepository.AddAsync(userSession);
            }
            else
            {
                userSession.RefreshToken = hashedRefreshToken;
                userSession.LastUpdatedAt = DateTime.UtcNow;
                userSession.IsRevoked = false;

                _unitOfWork.UserSessionRepository.Update(userSession);
            }

            //mark user as verified
            codeModel.User.IsVerified = true;

            //save change to database
            await _unitOfWork.CompleteAsync();

            //return response
            var response = _mapper.Map<LoginResponse>(codeModel.User);
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            return new ApiResponse<LoginResponse>(response);
        }
        #endregion

        #region ForgetPasswordRequestWithBackupEmailAsync
        public Task<ApiResponse<object>> ForgetPasswordRequestWithBackupEmailAsync(ForgetPasswordRequestWithBackupEmailDto request, string deviceId)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
