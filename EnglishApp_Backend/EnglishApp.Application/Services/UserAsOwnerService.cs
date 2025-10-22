using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Common;
using EnglishApp.Application.Common.Requests;
using EnglishApp.Application.DTOs.UserDto.User;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Domain.Common;

namespace EnglishApp.Application.Services
{
    public class UserAsOwnerService : IUserAsOwnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserAsOwnerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region GetAllUserAsync
        public async Task<ApiResponse<IEnumerable<UsersResponse>>> GetAllUserAsync(QueryParameters query)
        {
            var userList = await _unitOfWork.UserRepository.GetAllAsNoTrackingAsync();

            var response = _mapper.Map<IEnumerable<UsersResponse>>(userList);
            return new ApiResponse<IEnumerable<UsersResponse>>(response);

        }
        #endregion

        #region GetAllUserWithSoftDeleteAsync
        public async Task<ApiResponse<IEnumerable<UsersWithSoftDeleteResponse>>> GetAllUserWithSoftDeleteAsync(QueryParameters query)
        {
            var userList = await _unitOfWork.UserRepository.GetAllWithSoftDeleteAsync();

            var response = _mapper.Map<IEnumerable<UsersWithSoftDeleteResponse>>(userList);
            return new ApiResponse<IEnumerable<UsersWithSoftDeleteResponse>>(response);
        }
        #endregion

        #region ChangeStatusAsync
        public async Task<ApiResponse<object>> ChangeStatusAsync(int id, ChangeStatusQuery query)
        {
            //get user info 
            var user = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.UserId == id, "UserSessions");
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            if (query.IsSoftDeleted == null)
            {
                return new ApiResponse<object>("You must provide the 'isSoftDelete' value.");
            }

            if (query.IsSoftDeleted == true)
            {
                _unitOfWork.UserSessionRepository.RemoveRange(user.UserSessions);
            }

            //change status of user 
            user.IsDeleted = query.IsSoftDeleted.Value;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CompleteAsync();

            var message = query.IsSoftDeleted.Value ? "The user has been deactivated successfully." : "The user has been activated successfully.";
            var response = _mapper.Map<UsersWithSoftDeleteResponse>(user);
            return new ApiResponse<object>(response, message);
        }
        #endregion

        #region DeleteUserAsync
        public async Task<ApiResponse<object>> DeleteUserAsync(int id)
        {
            //get user info 
            var user = await _unitOfWork.UserRepository.FirstOrDefaultWithSoftDeleteAsync(u => u.UserId == id, "UserSessions");
            if (user == null)
            {
                return new ApiResponse<object>("User Not Found.");
            }

            //delete user form database 
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.UserSessionRepository.RemoveRange(user.UserSessions);
            await _unitOfWork.CompleteAsync();

            //return response message
            return new ApiResponse<object>(true, null, "User has been deleted successfully (hard delete).", null);
        }
        #endregion
    }
}
