using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.User;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region CreateUserAsync
        public async Task<ApiResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //get userType from database
                var existUserType = await _unitOfWork.UserTypeRepository.FirstOrDefaultAsync(ut => ut.UserTypeName == "user");
                if (existUserType == null)
                {
                    return new ApiResponse<CreateUserResponse>("Role 'user' is not defined.");
                }

                //check if email is available
                var existEmail = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Email == request.Email);
                if (existEmail != null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new ApiResponse<CreateUserResponse>("The email address is already registered. Please use a different email address.");
                }

                //check if username is available
                var existUsername = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Username == request.Username);
                if (existUsername != null)
                {
                    await _unitOfWork.RollbackAsync();
                    return new ApiResponse<CreateUserResponse>("The username is already taken. Please choose a different username.");
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

                //save new user in the database
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();

                //return response
                var response = _mapper.Map<CreateUserResponse>(userModel);
                return new ApiResponse<CreateUserResponse>(response, "User has been created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return new ApiResponse<CreateUserResponse>(ex.Message);
            }
        }
        #endregion
    }
}
