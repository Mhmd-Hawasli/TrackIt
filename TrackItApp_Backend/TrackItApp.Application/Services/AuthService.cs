using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Repositories;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }


        #region RegisterAsync
        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, string currentDeviceId)
        {
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
                var existEmail = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Email == request.Email);
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

                //Map request to New UserModel
                var userModel = _mapper.Map<User>(request);
                userModel.CreatedAt = DateTime.UtcNow;
                userModel.IsDeleted = false;
                userModel.IsVerified = false;
                userModel.PasswordHash = request.Password;
                userModel.UserType = existUserType;
                await _unitOfWork.UserRepository.AddAsync(userModel);

                //send email verification
                await _emailService.SendEmailVerificationCode(userModel, currentDeviceId, CodeType.ActivateAccount);

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
        public Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, string currentDeviceId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region VerifyAccountCodeAsync
        public Task<ApiResponse<VerifyAccountResponse>> VerifyAccountCodeAsync(VerifyAccountRequest request, string currentDeviceId)
        {
            throw new NotImplementedException();
        }

        #endregion
        public Task<ApiResponse<object>> ResendCodeAsync(ResendCodeRequest request, string currentDeviceId)
        {
            throw new NotImplementedException();
        }
    }
}
