using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EnglishApp.Application.Common;
using EnglishApp.Application.Interfaces;


namespace EnglishApp.API.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        #region InvokeAsync
        public async Task InvokeAsync(HttpContext context, IUnitOfWork _unitOfWork)
        {
            try
            {
                if (context.Request.Path.StartsWithSegments("/swagger"))
                {
                    await _next(context);
                    return;
                }
                // Get DeviceId and save it
                if (!context.Request.Headers.TryGetValue("Device-Id", out var deviceIds) || string.IsNullOrWhiteSpace(deviceIds))
                {
                    await WriteErrorResponse(context, 400, "Request header 'Device-Id' is missing.");
                    return;
                }
                string deviceId = deviceIds.ToString().ToLower();
                context.Items["DeviceId"] = deviceId;


                // pass [AllowAnonymous] from this middleware
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                {
                    await _next(context);
                    return;
                }


                //get token form header
                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
                    !authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Token missing.");
                    return;
                }


                // check token's valid and get user id
                var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Invalid token.");
                    return;
                }
                context.Items["UserId"] = userId;


                //get user session base on user id 
                var session = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsNoTrackingAsync(us => us.UserId == userId && us.DeviceId == deviceId, "User");
                if (session == null || session.User == null)
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: No active session found.");
                    return;
                }


                //check session is valid
                if (session.IsRevoked == true || session.LastUpdatedAt < DateTime.UtcNow.AddMonths(-2))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Invalid session.");
                    return;
                }


                //check if user is valid
                if (session.User.IsVerified == false || session.User.IsDeleted == true)
                {
                    var message = session.User.IsDeleted
                        ? "Unauthorized: User account is deleted."
                        : "Unauthorized: User not verified.";
                    await WriteErrorResponse(context, 401, message);
                    return;
                }


                ////////////////////////////
                //every thing is ok continue 
                await _next(context);
                ////////////////////////////
                return;
            }
            /////////////////////////
            //Global error handling//
            /////////////////////////
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>(false, null, null, [error]);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
                return;
            }
        }
        #endregion

        #region (private) WriteErrorResponse
        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message, int? data = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<int?>(false, data, message, null);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
        #endregion

    }
}
