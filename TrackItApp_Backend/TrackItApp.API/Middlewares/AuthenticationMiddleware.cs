using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TrackItApp.Application.Common;
using TrackItApp.Application.Interfaces;

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
                var session = await _unitOfWork.UserSessionRepository.FirstOrDefaultAsNoTrackingAsync(us => us.UserID == userId && us.DeviceID == deviceId, "User");
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
            catch (Exception ex)
            {
                await WriteErrorResponse(context, 401, $"Unauthorized: {ex.Message}");
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

        //#region (private) ValidateToken
        //private ClaimsPrincipal? ValidateToken(HttpContext context)
        //{
        //    // تحقق من وجود التوكن
        //    if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
        //        !authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return null; // ارجع null إذا كان التوكن مفقودًا أو غير صالح
        //    }

        //    try
        //    {
        //        // استخرج التوكن
        //        var token = authHeader.ToString()["Bearer ".Length..].Trim();

        //        // احصل على المفتاح السري من الإعدادات
        //        var signingKey = context.RequestServices.GetRequiredService<IConfiguration>()["JWT:SigningKey"];
        //        var key = Encoding.UTF8.GetBytes(signingKey!);

        //        var tokenHandler = new JwtSecurityTokenHandler();

        //        // إعدادات التحقق
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            ValidIssuer = context.RequestServices.GetRequiredService<IConfiguration>()["JWT:Issuer"],
        //            ValidAudience = context.RequestServices.GetRequiredService<IConfiguration>()["JWT:Audience"],
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ClockSkew = TimeSpan.Zero
        //        };

        //        // حاول التحقق من التوكن
        //        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        //        return principal;
        //    }
        //    catch (Exception)
        //    {
        //        // ارجع null إذا فشل التحقق من التوكن
        //        return null;
        //    }
        //}
        //#endregion

    }
}
