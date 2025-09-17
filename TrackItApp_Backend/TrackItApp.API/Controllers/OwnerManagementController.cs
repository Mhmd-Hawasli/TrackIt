using Microsoft.AspNetCore.Mvc;
using TrackItApp.Application.Common;
using TrackItApp.Application.Common.Requests;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.User;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Application.Services;
using TrackItApp.Domain.Common;

namespace TrackItApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerManagementController : ControllerBase
    {
        private readonly IUserAsOwnerService _ownerService;
        private readonly IUserService _userService;
        public OwnerManagementController(IUserAsOwnerService ownerService, IUserService userService)
        {
            _ownerService = ownerService;
            _userService = userService;
        }

        #region GetUserById
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var result = await _userService.GetUserInfoAsync(id);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region GetAllUser
        [HttpPost("getall")]
        [ProducesResponseType(typeof(ApiResponse<UsersResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllUser([FromBody] QueryParameters query)
        {
            var result = await _ownerService.GetAllUserAsync(query);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region GetAllUserWithSoftDelete
        [HttpPost("softDelete/getall")]
        [ProducesResponseType(typeof(ApiResponse<UsersWithSoftDeleteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllUserWithSoftDelete([FromBody] QueryParameters query)
        {
            var result = await _ownerService.GetAllUserWithSoftDeleteAsync(query);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region UpdateUser
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, [FromRoute] int id)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(request, id);
                if (!result.Succeeded)
                {
                    if (result.Message == "User Not Found.")
                        return NotFound(result);
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(ex.Message));
            }
        }
        #endregion

        #region change-status
        [HttpDelete("change-status/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeStatus([FromRoute] int id, [FromQuery] ChangeStatusQuery query)
        {
            var result = await _ownerService.ChangeStatusAsync(id, query);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region DeleteUser
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var result = await _ownerService.DeleteUserAsync(id);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }
}
