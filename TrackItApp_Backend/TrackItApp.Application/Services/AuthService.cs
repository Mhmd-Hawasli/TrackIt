using AutoMapper;
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
                var existEmail = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Email.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase));
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
                    u => u.Username == request.Input || u.Email.Equals(request.Input, StringComparison.CurrentCultureIgnoreCase),
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
                if (session == null || session.LastUpdatedAt < DateTime.Now.AddMonths(-2))
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
                var refreshToken = _tokenService.GenerateRefreshToken();

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
        public async Task<ApiResponse<object>> ResendCodeAsync(ResendCodeRequest request, string currentDeviceId)
        {
            try
            {
                //get code record from database via Email and DeviceID
                var verificationCode = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(vc => vc.Email.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase) && vc.DeviceID == currentDeviceId);
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
        public async Task<ApiResponse<LoginResponse>> VerifyAccountCodeAsync(VerifyAccountRequest request, string currentDeviceId)
        {
            try
            {
                // check if user enter email address
                if (string.IsNullOrEmpty(request.Email))
                {
                    return new ApiResponse<LoginResponse>("Please enter your email.");
                }

                //get verification code record from database
                var code = await _unitOfWork.VerificationCodeRepository.FirstOrDefaultAsync(
                    vc => vc.DeviceID == currentDeviceId && vc.Email.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase),
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
                var refreshToken = _tokenService.GenerateRefreshToken();

                //save or update user session in database
                UserSession? userSession = code.User.UserSessions.FirstOrDefault(us => us.DeviceID == currentDeviceId);
                if (userSession == null)
                {
                    userSession = new UserSession()
                    {
                        RefreshToken = refreshToken,
                        DeviceID = currentDeviceId,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdatedAt = DateTime.UtcNow,
                        UserID = code.UserID
                    };
                    await _unitOfWork.UserSessionRepository.AddAsync(userSession);
                }
                else
                {
                    userSession.RefreshToken = refreshToken;
                    userSession.LastUpdatedAt = DateTime.UtcNow;

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
    }
}
