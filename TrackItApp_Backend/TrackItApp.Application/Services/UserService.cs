using AutoMapper;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.User;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;

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

        #region GetUserInfoAsync
        public async Task<ApiResponse<GetUserResponse>> GetUserInfoAsync(int userId)
        {
            try
            {
                //get user info
                var user = await _unitOfWork.UserRepository.FirstOrDefaultAsNoTrackingAsync(u => u.UserID == userId);
                if (user == null)
                {
                    return new ApiResponse<GetUserResponse>("User Not Found.");
                }

                
                var response = _mapper.Map<GetUserResponse>(user);
                return new ApiResponse<GetUserResponse>(response);
            }
            catch 
            {
                throw;
            }
        } 
        #endregion
    }
}
