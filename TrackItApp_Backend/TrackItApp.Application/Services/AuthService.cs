using AutoMapper;
using System.Security.Claims;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
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
            //2- send email and save code in database
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
                return new ApiResponse<RegisterResponse>(response, "User has been created successfully.");
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
            try
            {
                // get user info 
                var user = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(
                    u => u.Username == request.Input || u.Email == request.Input.ToLower(),
                    "UserType");
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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
            catch
            {
                throw;
            }
        }
        #endregion

        #region ResendCodeAsync
        public async Task<ApiResponse<object>> ResendCodeAsync(ResendCodeDto request, string currentDeviceId)
        {
            try
            {
                //get code record from database via Email and DeviceID
                var verificationCode = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.User.Email == request.Email.ToLower() && vc.DeviceID == currentDeviceId, "User");
                if (verificationCode == null || verificationCode.CodeType != request.CodeType)
                {
                    return new ApiResponse<object>("You don’t have any expired code to resend.");
                }

                await _emailService.SendEmailVerificationCode(verificationCode.UserID, request.Email, currentDeviceId, request.CodeType);
                await _unitOfWork.CompleteAsync();

                return new ApiResponse<object>(true, null, "The code has been re-sent to your email.", null);
            }
            catch
            {
                throw;
            }

        }
        #endregion

        #region VerifyAccountCodeAsync
        public async Task<ApiResponse<LoginResponse>> VerifyAccountCodeAsync(VerifyAccountDto request, string currentDeviceId)
        {
            try
            {
                //get verification code record from database
                var code = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(
                    vc => vc.DeviceID == currentDeviceId && vc.User.Email == request.Email.ToLower(),
                    "User.UserSessions", "User.UserType");
                if (code == null)
                {
                    return new ApiResponse<LoginResponse>("There is no active code for this device.");
                }

                //check if code is correct and it has the same type
                if (code.Code != request.Code && code.CodeType != CodeType.ActivateAccount)
                {
                    return new ApiResponse<LoginResponse>("The verification code you entered is invalid.");
                }

                //check if code is expired and new email
                if (code.ExpiresAt < DateTime.UtcNow)
                {
                    await _emailService.SendEmailVerificationCode(code.UserID, request.Email, currentDeviceId, CodeType.ActivateAccount);
                    await _unitOfWork.CompleteAsync();
                    return new ApiResponse<LoginResponse>("Your code has expired. Please check your email address.");
                }

                //remove the code form database if everything is ok 
                _unitOfWork.VerificationCodeRepository.Remove(code);

                //generate accessToken and refreshToken
                var accessToken = _tokenService.CreateToken(code.User);
                (string refreshToken, string hashedRefreshToken) = _tokenService.GenerateRefreshToken();

                //save or update user session in database
                UserSession? userSession = code.User.UserSessions.FirstOrDefault(us => us.DeviceID == currentDeviceId);
                if (userSession == null)
                {
                    userSession = new UserSession()
                    {
                        RefreshToken = hashedRefreshToken,
                        DeviceID = currentDeviceId,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                        UserID = code.UserID,
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
                code.User.IsVerified = true;

                //save change to database
                await _unitOfWork.CompleteAsync();

                //return response
                var response = _mapper.Map<LoginResponse>(code.User);
                response.AccessToken = accessToken;
                response.RefreshToken = refreshToken;
                return new ApiResponse<LoginResponse>(response);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region LogoutAsync
        public async Task<ApiResponse<object>> LogoutAsync(int userId, string currentDeviceId)
        {
            try
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
            catch
            {
                throw;
            }
        }
        #endregion

        #region UpdateTokenAsync
        public async Task<ApiResponse<UpdateTokenResponse>> UpdateTokenAsync(UpdateTokenRequest request, string currentDeviceId)
        {
            try
            {
                //check token's secret key
                var token = _tokenService.ValidateExpiredAccessToken(request.AccessToken);

                if (token == null)
                {
                    return new ApiResponse<UpdateTokenResponse>("Your access token is invalid. Please log in again.");
                }

                //get UserID form token
                var userIdString = token.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return new ApiResponse<UpdateTokenResponse>("Your access token is invalid. Please log in again.");
                }

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
            catch
            {
                throw;
            }
        }
        #endregion


    }
}
