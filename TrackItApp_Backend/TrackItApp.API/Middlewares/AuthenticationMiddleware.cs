using System.Text.Json;                        // JsonSerializer
using TrackItApp.Application.Common;           // ApiResponse
using Microsoft.AspNetCore.Http;               // HttpContext, RequestDelegate
using Microsoft.AspNetCore.Authorization;     // IAllowAnonymous
using Microsoft.AspNetCore.Routing;           // Endpoint, GetEndpoint()
using Microsoft.IdentityModel.Tokens;         // SecurityTokenExpiredException, SymmetricSecurityKey
using Microsoft.Extensions.Configuration;      // IConfiguration
using System.IdentityModel.Tokens.Jwt;        // JwtSecurityTokenHandler, JwtSecurityToken
using System.Security.Claims;                  // ClaimTypes
using System.Text;                             // Encoding.UTF8
using System.Threading.Tasks;                  // Task
using System.Linq;                             // FirstOrDefault(), Last()
using System;
using TrackItApp.Application.Interfaces;                                  // Exception, InvalidOperationException

namespace TrackItApp.API.Middlewares
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
                // Get DeviceId and save it
                if (!context.Request.Headers.TryGetValue("Device-Id", out var deviceIds) || string.IsNullOrWhiteSpace(deviceIds))
                {
                    await WriteErrorResponse(context, 400, "Request header 'Device-Id' is missing.");
                    return;
                }
                var deviceId = deviceIds.ToString().ToLower();
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


                //get user session base on user id 
                var session = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsNoTrackingAsync(us => us.UserID == userId && us.DeviceID == deviceId);
                if (session == null)
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: No active session found.");
                    return;
                }


                //check session is valid
                if (session.IsRevoked == false || session.LastUpdatedAt < DateTime.UtcNow.AddMonths(-2))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Invalid session.");
                    return;
                }



                //every thing is ok continue 
                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                await WriteErrorResponse(context, 401, "Token is expired", 1);
                return;
            }
            catch (Exception ex)
            {
                await WriteErrorResponse(context, 401, $"Unauthorized: {ex.Message}");
                return;
            }
        }
        #endregion

        #region (private) ValidateToken
        //private ClaimsPrincipal ValidateToken(string token)
        //{
        //    var signingKey = _configuration["JWT:SigningKey"]
        //                     ?? throw new InvalidOperationException("JWT:SigningKey is missing");
        //    var key = Encoding.UTF8.GetBytes(signingKey);

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var validationParams = new TokenValidationParameters
        //    {
        //        ClockSkew = TimeSpan.Zero,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        ValidateIssuer = false,
        //        //ValidIssuer = _configuration["JWT:Issuer"],
        //        ValidateAudience = false,
        //        //ValidAudience = _configuration["JWT:Audience"],
        //        ValidateLifetime = false,
        //    };
        //    var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);

        //    // ✅ signature صالح
        //    return principal;
        //}
        private async Task<ClaimsPrincipal?> ValidateToken(HttpContext context, string token)
        {
            try
            {
                var signingKey = _configuration["JWT:SigningKey"]
                                 ?? throw new InvalidOperationException("JWT:SigningKey is missing");

                var key = Encoding.UTF8.GetBytes(signingKey);
                var symmetricKey = new SymmetricSecurityKey(key);

                var tokenHandler = new JwtSecurityTokenHandler();

                // ⚡ إعدادات التحقق
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = symmetricKey,

                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],

                    ValidateLifetime = true,
                    //ClockSkew = TimeSpan.Zero, // بدون سماحية زمنية
                };

                var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);

                // ✅ التوكن صالح
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token expired.");
                return null;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token signature invalid.");
                return null;
            }
            catch (SecurityTokenInvalidLifetimeException)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token lifetime invalid (exp/nbf).");
                return null;
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token audience invalid.");
                return null;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token issuer invalid.");
                return null;
            }
            catch (Exception)
            {
                await WriteErrorResponse(context, 401, "Unauthorized: Token invalid.");
                return null;
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
