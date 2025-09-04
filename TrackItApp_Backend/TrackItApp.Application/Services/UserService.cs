using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.User;
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
                //get userType
                var userType = await _unitOfWork.UserTypeRepository.FirstOrDefaultAsNoTrackingAsync(ut => ut.UserTypeName == "user");
                if (userType == null)
                {
                    return new ApiResponse<CreateUserResponse>("Role 'user' is not defined.");
                }
                var userModel = _mapper.Map<User>(request);
                userModel.CreatedAt = DateTime.UtcNow;
                userModel.IsDeleted = false;
                userModel.IsVerified = false;
                userModel.IsActive = true;
                userModel.UserType = userType;

                throw new NotImplementedException();
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
