using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Application.Common;
using EnglishApp.Application.Common.Requests;
using EnglishApp.Application.DTOs.UserDto.User;
using EnglishApp.Domain.Common;

namespace EnglishApp.Application.Interfaces.Services
{
    public interface IUserAsOwnerService
    {
        Task<ApiResponse<IEnumerable<UsersResponse>>> GetAllUserAsync(QueryParameters query);
        Task<ApiResponse<IEnumerable<UsersWithSoftDeleteResponse>>> GetAllUserWithSoftDeleteAsync(QueryParameters query);
        Task<ApiResponse<object>> ChangeStatusAsync(int id, ChangeStatusQuery query);
        Task<ApiResponse<object>> DeleteUserAsync(int id);

    }
}
