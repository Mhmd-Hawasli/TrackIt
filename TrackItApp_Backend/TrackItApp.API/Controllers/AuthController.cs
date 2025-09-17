using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.Auth.ChangeEmail;
using TrackItApp.Application.DTOs.UserDto.Auth.ChangePassword;
using TrackItApp.Application.Interfaces.Services;


namespace TrackItApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        #region register

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;


            var result = await _authService.RegisterAsync(request, deviceId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region login
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.LoginAsync(request, deviceId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region resend-activate-code
        [HttpPost("resend-activate-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendActivateCode([FromBody] ResendActivateCodeDto request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ResendActivateCodeAsync(request, deviceId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion 

        #region verify-activate-code
        [HttpPost("verify-activate-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyActivateCode([FromBody] VerifyActivateDto request)
        {

            //print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.VerifyActivateCodeAsync(request, deviceId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region logout
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            //get UserID form token
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.LogoutAsync(userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
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

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.UpdateTokenAsync(request, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "Your access token is invalid. Please log in again.")
                    return Unauthorized(result);
                return BadRequest(result);
            }
            return Ok(result);
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
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ForgetPasswordRequestAsync(request, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
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
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ForgetPasswordVerifyCodeAsync(request, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
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
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ForgetPasswordResetPasswordAsync(request, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region change-password
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse<UpdateTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            //get userId form token
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            var result = await _authService.ChangePasswordAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region change-email/request
        [HttpPost("change-email/request")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeEmailRequest([FromBody] ChangeEmailRequest request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ChangeEmailRequestAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region change-email/verify
        [HttpPost("change-email/verify")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeEmailVerify([FromBody] ChangeEmailVerify request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ChangeEmailVerifyAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }

}

