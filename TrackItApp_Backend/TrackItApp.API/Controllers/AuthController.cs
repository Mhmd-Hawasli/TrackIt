using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TrackItApp.Application.Common;
using TrackItApp.Application.DTOs.UserDto.Auth;
using TrackItApp.Application.DTOs.UserDto.Auth.AccountActivation;
using TrackItApp.Application.DTOs.UserDto.Auth.BackupEmail;
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


        //-----------------------------
        // Authentication
        //-----------------------------

        //register
        #region register
        /// <summary>
        /// Registers a new user account using the provided registration data,
        /// and associates the account with the current device ID from the request header.
        /// After successful registration, the user will be required to confirm their account,
        /// and a verification code will be sent to their contact (e.g., email).
        /// </summary>
        /// <param name="request">The registration details provided by the client (e.g., name, username, email, password).</param>
        /// <returns>
        /// Returns 200 OK with the registration result if successful, 
        /// or 400 Bad Request with error details if validation fails or registration is unsuccessful.
        /// </returns>
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

        //login
        #region login
        /// <summary>
        /// Validates the user's login credentials (username or email and password) 
        /// together with the current device ID.
        /// If a valid and confirmed session already exists, the user is logged in 
        /// and receives an access token along with a refresh token.
        /// If no valid session exists, or the account has not yet been confirmed, 
        /// a verification code will be sent to the user’s primary email for account activation.
        /// Note: the actual session creation occurs in the activate-account/verify endpoint.
        /// </summary>
        /// <param name="request">The login details provided by the client (username or email, and password).</param>
        /// <returns>
        /// Returns 200 OK with access and refresh tokens if a valid session exists, 
        /// or 400 Bad Request with error/verification details if login cannot proceed.
        /// </returns>
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

        //logout
        #region logout
        /// <summary>
        /// Logs out the currently authenticated user on the given device.
        /// This operation ends the active session associated with the user and device, 
        /// ensuring that any further requests using the same session will no longer be valid.
        /// </summary>
        /// <returns>
        /// Returns 200 OK if the logout process succeeds, 
        /// 404 Not Found if the user or session cannot be found, 
        /// or 400 Bad Request if the operation fails for another reason.
        /// </returns>
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

        //update-token
        #region update-token
        /// <summary>
        /// Refreshes the user's authentication tokens.
        /// The client sends an expired access token along with a valid refresh token.
        /// The system validates the expired access token (ignoring expiration) to extract the user ID,
        /// verifies the session for the given device, and ensures the refresh token matches.
        /// If valid, a new access token and refresh token are issued, 
        /// and the session's validity is updated (default maximum lifetime: 2 months).
        /// </summary>
        /// <param name="request">
        /// The request containing the expired access token and the refresh token to validate and refresh the session.
        /// </param>
        /// <returns>
        /// Returns 200 OK with a new access token and refresh token if the operation succeeds,  
        /// 401 Unauthorized if the access token is invalid and login is required,  
        /// or 400 Bad Request if the refresh token/session is invalid.
        /// </returns>
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

        //account-activations/resend
        #region account-activations/resend
        /// <summary>
        /// Resends the account activation verification code for the user.
        /// If a previous code exists in the database, it will be invalidated and replaced with a new code.
        /// The new code is then sent again to the user’s registered contact (e.g., email).
        /// </summary>
        /// <param name="request">The request containing the necessary user information to resend the activation code.</param>
        /// <returns>
        /// Returns 200 OK if a new activation code was generated and sent successfully, 
        /// or 400 Bad Request with error details if the operation fails.
        /// </returns>
        [HttpPost("account-activations/resend")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendActivationCode([FromBody] ResendActivationCodeDto request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ResendActivationCodeAsync(request, deviceId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        //account-activations/verify
        #region account-activations/verify
        /// <summary>
        /// Verifies the activation code sent to the user after registration or login.
        /// If a session already exists for the device, it will be updated; 
        /// otherwise, a new session will be created.
        /// Once verified, the user receives an access token along with a refresh token,
        /// and the activation code is removed from the database since the account is now confirmed.
        /// </summary>
        /// <param name="request">The request containing the activation code and related user/device details.</param>
        /// <returns>
        /// Returns 200 OK with access and refresh tokens if verification succeeds, 
        /// or 400 Bad Request with error details if verification fails.
        /// </returns>
        [HttpPost("account-activations/verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyActivationCode([FromBody] VerifyActivationCodeDto request)
        {

            //print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.VerifyActivationCodeAsync(request, deviceId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion


        //-----------------------------
        // Password
        //-----------------------------

        //forgot-password/request
        #region forgot-password/request
        /// <summary>
        /// Initiates the forgot password process by sending a verification code to the user's primary email.
        /// The user provides their email, and if the account exists, a reset code is generated and sent to the primary email.
        /// This code will be used later to verify the user before allowing a password reset.
        /// </summary>
        /// <param name="request">The request containing the user's email address for password reset.</param>
        /// <returns>
        /// Returns 200 OK if the verification code was successfully sent,  
        /// 404 Not Found if the user with the given email does not exist,  
        /// or 400 Bad Request if the operation fails for another reason.
        /// </returns>
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

        //forgot-password/verify
        #region forgot-password/verify
        /// <summary>
        /// Verifies the password reset code sent to the user's primary email.
        /// This is an intermediate step in the forgot password process to ensure the provided code is valid and not expired.
        /// The code must match the one previously sent for password reset, and it is checked against the current device ID.
        /// </summary>
        /// <param name="request">The request containing the user's email and the verification code received.</param>
        /// <returns>
        /// Returns 200 OK if the verification code is valid and the user can proceed to the next step,  
        /// 404 Not Found if the user or verification record cannot be found,  
        /// or 400 Bad Request if the verification code is invalid or expired.
        /// </returns>
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

        //forgot-password/reset
        #region forgot-password/reset
        /// <summary>
        /// Resets the user's password after successful verification of the reset code.
        /// This is the final step in the forgot password process. The new password is validated, 
        /// must be different from the old password, and then updated in the user's account.
        /// All other active sessions on different devices are removed, 
        /// and the verification code is deleted from the database.
        /// </summary>
        /// <param name="request">The request containing the user's email, the verified code, and the new password.</param>
        /// <returns>
        /// Returns 200 OK if the password was successfully reset,  
        /// 404 Not Found if the user or verification record cannot be found,  
        /// or 400 Bad Request if the verification code is invalid or the new password does not meet requirements.
        /// </returns>
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

        //change-password
        #region change-password
        /// <summary>
        /// Changes the authenticated user's password from within the system.
        /// The user must provide the current (old) password and a new password.
        /// Upon successful change, all other active sessions on different devices are removed,
        /// while keeping the current session active. A valid access token is required to perform this action.
        /// </summary>
        /// <param name="request">The request containing the old password and the new desired password.</param>
        /// <returns>
        /// Returns 200 OK if the password was successfully changed,  
        /// 404 Not Found if the user or verification record cannot be found,
        /// or 400 Bad Request if the old password is incorrect or the new password is invalid.
        /// </returns>
        [HttpPut("change-password")]
        [ProducesResponseType(typeof(ApiResponse<UpdateTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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


        //-----------------------------
        // Change Email
        //-----------------------------

        //change-email/request
        #region change-email/request
        /// <summary>
        /// Starts the process to change the authenticated user's email address.
        /// This endpoint requires a valid access token and the user's current password for security.
        /// It checks that the user exists, verifies the current password, and ensures the new email is not the same as the current or backup email, and not already used by another user.
        /// If everything is valid, a verification code is sent to the new email address.
        /// The response indicates whether the verification email was sent successfully or if there was an error.
        /// </summary>
        /// <param name="request">The request containing the current password and the new email address.</param>
        /// <returns>
        /// - 200 OK: Verification email sent successfully.
        /// - 400 BadRequest: Validation failed or business rule violation, such as incorrect password or email already in use.
        /// - 404 NotFound: User not found.
        /// </returns>
        [HttpPost("change-email/request")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestChangeEmail([FromBody] RequestChangeEmailDto request)
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

            var result = await _authService.RequestChangeEmailAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion 

        //change-email/verify
        #region change-email/verify
        /// <summary>
        /// Verifies the email change request for the authenticated user.
        /// This endpoint requires a valid access token and the verification code sent to the new email.
        /// It checks that the verification code is valid, not expired, and matches the new email.
        /// Once verified, the user's email is updated, all sessions on other devices are removed to keep only the current session active,
        /// and the verification code is deleted from the database.
        /// The response confirms whether the email change was successful or if there was an error.
        /// </summary>
        /// <param name="request">The request containing the new email address and the verification code received via email.</param>
        /// <returns>
        /// - 200 OK: Email changed successfully.
        /// - 400 BadRequest: Verification code is invalid or other validation errors.
        /// - 404 NotFound: User not found.
        /// </returns>
        [HttpPost("change-email/verify")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyChangeEmail([FromBody] VerifyChangeEmailDto request)
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

            var result = await _authService.VerifyChangeEmailAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion 


        //-----------------------------
        // Change Email
        //-----------------------------

        //backup-email/request-add
        #region backup-email/request-add
        [HttpPost("backup-email/request-add")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestAddBackupEmail([FromBody] RequestAddBackupEmailDto request)
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

            var result = await _authService.RequestAddBackupEmailAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        //backup-email/verify-add
        #region backup-email/verify-add
        [HttpPost("backup-email/verify-add")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyAddBackupEmail([FromBody] VerifyAddBackupEmailDto request)
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

            var result = await _authService.VerifyAddBackupEmailAsync(request, userId, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        //backup-email/remove
        #region backup-email/remove
        [HttpDelete("backup-email/remove")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveBackupEmail()
        {
            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            var result = await _authService.RemoveBackupEmailAsync(userId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        //backup-email/account-activations/request
        #region backup-email/account-activations/request
        [AllowAnonymous]
        [HttpPost("backup-email/account-activations/request")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RequestActivationWithBackupEmail(RequestActivationWithBackupEmailDto request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.RequestActivationWithBackupEmailAsync(request, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion 

        //backup-email/account-activations/verify
        #region backup-email/account-activations/verify
        [AllowAnonymous]
        [HttpPost("backup-email/account-activations/verify")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyActivationWithBackupEmail([FromBody] VerifyActivationWithBackupEmailDto request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            
            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.VerifyActivationWithBackupEmailAsync(request, deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok();
        }
        #endregion

        //backup-email/forgot-password/request
        #region backup-email/forgot-password/request 
        [AllowAnonymous]
        [HttpPost("backup-email/forgot-password/request")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgetPasswordRequestWithBackupEmail([FromBody]ForgetPasswordRequestWithBackupEmailDto request)
        {
            // print validation error
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(ModelState));
            }

            //get DeviceID form request header
            string deviceId = HttpContext.Items["DeviceId"]!.ToString()!;

            var result = await _authService.ForgetPasswordRequestWithBackupEmailAsync(request,deviceId);
            if (!result.Succeeded)
            {
                if (result.Message == "User not found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        //backup-email/forgot-password/verify
        #region backup-email/forgot-password/verify
        [HttpPost("backup-email/forgot-password/verify")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeEmailVerify12()
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

            //var result = await _authService.VerifyChangeEmailAsync(request, userId, deviceId);
            //if (!result.Succeeded)
            //{
            //    if (result.Message == "User not found.")
            //        return NotFound(result);
            //    return BadRequest(result);
            //}
            return Ok();
        }
        #endregion

        //backup-email/forgot-password/reset
        #region backup-email/forgot-password/reset
        [HttpPost("backup-email/forgot-password/reset")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeEmailVerify13()
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

            //var result = await _authService.VerifyChangeEmailAsync(request, userId, deviceId);
            //if (!result.Succeeded)
            //{
            //    if (result.Message == "User not found.")
            //        return NotFound(result);
            //    return BadRequest(result);
            //}
            return Ok();
        }
        #endregion 
    }

}

