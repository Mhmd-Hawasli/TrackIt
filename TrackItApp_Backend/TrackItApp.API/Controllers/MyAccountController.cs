using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.User;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Application.Services;

namespace TrackItApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyAccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public MyAccountController(IUserService userService)
        {
            _userService = userService;
        }


        #region GetUserInfo
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserInfo()
        {
            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            var result = await _userService.GetUserInfoAsync(userId);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region UpdateUser
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            var result = await _userService.UpdateUserAsync(request, userId);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region deactivate-user
        [HttpDelete("deactivate-user")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeactivateUser()
        {
            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            var result = await _userService.DeactivateUserAsync(userId);
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
