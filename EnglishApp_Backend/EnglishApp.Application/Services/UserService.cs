using AutoMapper;
using Org.BouncyCastle.Tls.Crypto;
using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.UserDto.User;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Services;

namespace EnglishApp.Application.Services
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


        #region GetUserInfoAsync
        public async Task<ApiResponse<UserGetByIdResponse>> GetUserInfoAsync(int userId)
        {
            //get user info
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new ApiResponse<UserGetByIdResponse>("User Not Found.");
            }


            var response = _mapper.Map<UserGetByIdResponse>(user);
            return new ApiResponse<UserGetByIdResponse>(response);
        }
        #endregion

        #region UpdateUserAsync
        public async Task<ApiResponse<object>> UpdateUserAsync(UpdateUserRequest request, int userId)
        {

            //check if user is in database 
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            //check if username available
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
            {
                var existUsername = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.Username == request.Username);
                if (existUsername != null)
                {
                    return new ApiResponse<object>("This username is already in use. Please choose another.");
                }
                else
                {
                    user.Username = request.Username;
                }

            }

            //update name
            if (!string.IsNullOrEmpty(request.Name))
            {
                user.Name = request.Name;
            }

            //update user in database
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "User has been updated successfully.", null);
        }
        #endregion

        #region DeactivateUserAsync
        public async Task<ApiResponse<object>> DeactivateUserAsync(int userId)
        {
            //get user info 
            var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(u => u.UserId == userId, "UserSessions");
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            //delete all user session form all device 
            _unitOfWork.UserSessionRepository.RemoveRange(user.UserSessions);

            //delete user form database (soft delete)
            _unitOfWork.UserRepository.Remove(user);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<object>(true, null, "The user account has been removed successfully.", null);
        }
        #endregion
    }
}
