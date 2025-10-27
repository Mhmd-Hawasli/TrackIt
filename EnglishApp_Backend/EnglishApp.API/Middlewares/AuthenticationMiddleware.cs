using EnglishApp.Application.Common;
using EnglishApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


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
                //pass if endpoint start with swagger
                if (context.Request.Path.StartsWithSegments("/swagger"))
                {
                    await _next(context);
                    return;
                }
                //=================================================


                // Get DeviceId and save it
                if (!context.Request.Headers.TryGetValue("Device-Id", out var deviceIds) || string.IsNullOrWhiteSpace(deviceIds))
                {
                    await WriteErrorResponse(context, 400, "Request header 'Device-Id' is missing.");
                    return;
                }
                string deviceId = deviceIds.ToString().ToLower();
                context.Items["DeviceId"] = deviceId;
                //=================================================


                // Check if endpoint allows anonymous
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                {
                    await _next(context);
                    return;
                }
                //=================================================


                // 1. Check Authorization header
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    await WriteErrorResponse(context, 401, "Missing or invalid Authorization header.");
                    return;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var signingKey = _configuration["JWT:SigningKey"];
                if (signingKey == null) throw new Exception();
                var signingKeyBytes = Encoding.UTF8.GetBytes(signingKey);

                // 2. Extract token
                var token = authHeader.Substring("Bearer ".Length).Trim();


                // 3. Set up validation parameters
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidateLifetime = true,
                },
                out SecurityToken validatedToken);
                context.User = principal;


                // 4. Extract userId from claims
                var userIdString = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Invalid token.");
                    return;
                }
                context.Items["UserId"] = userId;

                // 5. Get user session based on userId and deviceId
                var session = await _unitOfWork.UserSessionRepository
                    .FirstOrDefaultAsync(us => us.UserId == userId && us.DeviceId == deviceId, "User");

                if (session == null || session.User == null)
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: No active session found.");
                    return;
                }

                // 6. Validate session
                if (session.IsRevoked || session.LastUpdatedAt < DateTime.UtcNow.AddMonths(-2))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Invalid session.");
                    return;
                }

                // 7. Validate user
                if (!session.User.IsVerified || session.User.IsDeleted)
                {
                    var message = session.User.IsDeleted
                        ? "Unauthorized: User account is deleted."
                        : "Unauthorized: User not verified.";
                    await WriteErrorResponse(context, 401, message);
                    return;
                }

                // 8. Everything is ok, continue pipeline
                await _next(context);
                return;

            }
            /////////////////////////
            //Global error handling//
            /////////////////////////
            catch (SecurityTokenExpiredException)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token is expired.");
                return;
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
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
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<int?>(false, data, message, null);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
            }
        }
        #endregion

    }
}
