using Microsoft.AspNetCore.Mvc;
using TrackItApp.Application.DTOs.User;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Application.Common;

namespace TrackItApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        #region create-user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var result = await _userService.CreateUserAsync(request);
                if (result.Succeeded)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(ex.Message));
            }
        }
        #endregion
    }
}
