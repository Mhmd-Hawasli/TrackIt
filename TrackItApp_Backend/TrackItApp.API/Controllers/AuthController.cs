using Microsoft.AspNetCore.Mvc;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.Interfaces.Repositories;


namespace TrackItApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthController(IAuthService authService, IHttpContextAccessor contextAccessor)
        {
            _authService = authService;
            _contextAccessor = contextAccessor;
        }

        #region register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                //get DeviceID form request header
                var currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.RegisterAsync(request,currentDeviceId);
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
