
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using TrackItApp.Application.Common;
using TrackItApp.Application.Interfaces;

namespace MandoobApp.API.Middlewares
{
    public class VerificationMiddleware
    {
        private readonly RequestDelegate _next;

        public VerificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork _unitOfWork)
        {
            //check if there is a Token
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<String>("Unauthorized: Token missing.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }


            //Get User info from Token
            int userId;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out userId))
                {
                    throw new Exception("Invalid token");
                }
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<string>("Invalid token");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            // Get Device info from Request    
            var currentDeviceType = context.Request.Headers["Device-Type"].FirstOrDefault();
            var currentDeviceIdentifier = context.Request.Headers["Device-Identifier"].FirstOrDefault();


            //Get session form DataBase
            var session = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsync(us => us.UserID == userId);

            if (session == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<String>("Unauthorized: Session not found.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            //Session Date is Expired
            //if ((DateTime.UtcNow - session.LastUsedAt).TotalDays > 180)
            //{
            //    _unitOfWork.UserSessionRepository.Remove(session);
            //    await _unitOfWork.CompleteAsync();

            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    context.Response.ContentType = "application/json";
            //    var response = new ApiResponse<String>("Unauthorized: Session expired.");
            //    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            //    return;
            //}

            //check Device Identifier
            if (session.DeviceID != currentDeviceIdentifier)
            {
                _unitOfWork.UserSessionRepository.Remove(session);
                await _unitOfWork.CompleteAsync();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<String>("Unauthorized: Device mismatch.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            // Check if the session is unValid 
            //if (session.IsLogout)
            //{
            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    context.Response.ContentType = "application/json";
            //    var response = new ApiResponse<String>("Unauthorized: User logged out.");
            //    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            //    return;
            //}

            // ✅ Update LastUsedAt before continue
            //session.LastUsedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            //------------------------------------------- other middleware in test ------------------------------------------------

            // 1. Check if the user is authenticated.
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdString = context.User.FindFirst("UserID")?.Value;
                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId2))
                {
                    // 2. Get the user from the database.
                    var user = await _unitOfWork.UserRepository.GetByIdAsync(userId2);

                    // 3. Check if the user is not verified and the current path is not a verification path.
                    if (user != null && !user.IsVerified && !IsVerificationPath(context.Request.Path))
                    {
                        // 4. Return an unauthorized response.
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden; // Or Unauthorized (401)
                        await context.Response.WriteAsJsonAsync(new { message = "Your account is not verified. Please check your email or phone for the verification code." });
                        return;
                    }
                }
            }

            //check if user is verified

            // 5. If everything is fine, proceed to the next middleware.
            await _next(context);
        }

        private bool IsVerificationPath(PathString path)
        {
            // Define the paths that don't require verification (e.g., login, registration, and verification endpoint itself).
            return path.StartsWithSegments("/api/auth/login") ||
                   path.StartsWithSegments("/api/auth/register") ||
                   path.StartsWithSegments("/api/auth/verify");
        }
    }
}
