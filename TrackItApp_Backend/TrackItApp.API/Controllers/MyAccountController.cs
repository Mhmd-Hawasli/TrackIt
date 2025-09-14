using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
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
            try
            {
                //get userId from token 
                var userIdStrig = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdStrig == null || !int.TryParse(userIdStrig, out int userId))
                {
                    return BadRequest(new ApiResponse<object>("User is not authenticated or not found."));
                }

                var result = await _userService.GetUserInfoAsync(userId);
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



    }
}
