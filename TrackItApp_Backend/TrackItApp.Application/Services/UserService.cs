using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.User;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;

namespace TrackItApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region CreateUserAsync
        public Task<ApiResponse<CreateUserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
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
