using Microsoft.AspNetCore.Mvc;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.Interfaces.Services;


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


        //register
        #region register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                var currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.RegisterAsync(request, currentDeviceId);
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

        //login
        #region login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                var currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.LoginAsync(request, currentDeviceId);
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

        //resend-code
        #region resend-code
        [HttpPost("resend-code")]
        public async Task<IActionResult> ResendCode([FromBody] ResendCodeRequest request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                var currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.ResendCodeAsync(request, currentDeviceId);
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

        //verify-account-code
        #region verify-account-code
        [HttpPost("verify-account-code")]
        public async Task<IActionResult> VerifyAccountCode([FromBody] VerifyAccountRequest request)
        {
            try
            {
                //get DeviceID form request header
                var currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.VerifyAccountCodeAsync(request, currentDeviceId);
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

        //logout
        #region logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] VerifyAccountRequest request)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(ex.Message));
            }
        }
        #endregion

        //update-token
        #region update-token
        [HttpPost("update-token")]
        public async Task<IActionResult> UpdateToken([FromBody] VerifyAccountRequest request)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(ex.Message));
            }
        }
        #endregion



     



    }
}
