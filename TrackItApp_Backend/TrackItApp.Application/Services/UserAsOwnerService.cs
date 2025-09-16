using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.User;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Domain.Common;

namespace TrackItApp.Application.Services
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
            try
            {
                var userList = await _unitOfWork.UserRepository.GetAllAsNoTrackingAsync();

                var response = _mapper.Map<IEnumerable<UsersResponse>>(userList);
                return new ApiResponse<IEnumerable<UsersResponse>>(response);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region GetAllUserWithSoftDeleteAsync
        public async Task<ApiResponse<IEnumerable<UsersWithSoftDeleteResponse>>> GetAllUserWithSoftDeleteAsync(QueryParameters query)
        {
            try
            {
                var userList = await _unitOfWork.UserRepository.GetAllWithSoftDeleteAsync();

                var response = _mapper.Map<IEnumerable<UsersWithSoftDeleteResponse>>(userList);
                return new ApiResponse<IEnumerable<UsersWithSoftDeleteResponse>>(response);
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}
