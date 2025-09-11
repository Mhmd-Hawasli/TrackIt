using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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


        #region register

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.RegisterAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
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

        #region login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
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
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.LoginAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
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

        #region resend-code
        [HttpPost("resend-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendCode([FromBody] ResendCodeDto request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.ResendCodeAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
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

        #region verify-account-code
        [HttpPost("verify-account-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyAccountCode([FromBody] VerifyAccountDto request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }

                var result = await _authService.VerifyAccountCodeAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
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

        #region logout
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                //get UserID form token
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("User is not authenticated or not found.");
                }

                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }
                var result = await _authService.LogoutAsync(userId, currentDeviceId);
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

        #region update-token
        [HttpPost("update-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<UpdateTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateToken([FromBody] UpdateTokenRequest request)
        {
            try
            {
                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }


                var result = await _authService.UpdateTokenAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
                    if (result.Message == "Your access token is invalid. Please log in again.")
                        return Unauthorized(result);
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

        #region forgot-password/request
        [HttpPost("forgot-password/request")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgetPasswordRequest([FromBody] ForgetPasswordRequestDto request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }


                var result = await _authService.ForgetPasswordRequestAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
                    if (result.Message == "User not found.")
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

        #region forgot-password/verify
        [HttpPost("forgot-password/verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgetPasswordVerifyCode([FromBody] ForgetPasswordVerifyCodeDto request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }
                var result = await _authService.ForgetPasswordVerifyCodeAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
                    if (result.Message == "User not found.")
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

        #region forgot-password/reset
        [HttpPost("forgot-password/reset")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgetPasswordResetPassword([FromBody] ForgetPasswordResetPasswordDto request)
        {
            try
            {
                // print validation error
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>(ModelState));
                }

                //get DeviceID form request header
                string? currentDeviceId = _contextAccessor.HttpContext?.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    return BadRequest(new ApiResponse<object>("Request header 'Device-Id' is missing."));
                }
                var result = await _authService.ForgetPasswordResetPasswordAsync(request, currentDeviceId);
                if (!result.Succeeded)
                {
                    if (result.Message == "User not found.")
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

