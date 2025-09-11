using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TrackItApp.Application.Common;

namespace TrackItApp.API.Middleware
{
    public class Old_2
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public Old_2(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 1️⃣ التحقق من Device-Id
                if (!context.Request.Headers.TryGetValue("Device-Id", out var deviceIds) || string.IsNullOrWhiteSpace(deviceIds))
                {
                    await WriteErrorResponse(context, 400, "Request header 'Device-Id' is missing.");
                    return;
                }
                var deviceId = deviceIds.ToString().ToLower();

                // 2️⃣ السماح للحالات AllowAnonymous
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
                {
                    // للحالات بدون توكن
                    context.Items["DeviceId"] = deviceId;
                    await _next(context);
                    return;
                }

                // 3️⃣ التحقق من Authorization header
                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
                    !authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    await WriteErrorResponse(context, 401, "Unauthorized: Token missing.");
                    return;
                }

                var token = authHeader.ToString()["Bearer ".Length..].Trim();

                // 4️⃣ التحقق من JWT واستخراج Claims
                var claimsPrincipal = ValidateToken(token);

                // 5️⃣ إضافة DeviceId كـ Claim
                if (claimsPrincipal.Identity is ClaimsIdentity identity)
                {
                    identity.AddClaim(new Claim("DeviceId", deviceId));
                }

                // 6️⃣ ربط ClaimsPrincipal بـ HttpContext.User
                context.User = claimsPrincipal;

                // ✅ كل شيء تمام
                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                await WriteErrorResponse(context, 401, "Token is expired", 1);
            }
            catch
            {
                await WriteErrorResponse(context, 401, "Invalid token", 2);
            }
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            var signingKey = _configuration["JWT:SigningKey"]
                             ?? throw new InvalidOperationException("JWT:SigningKey is missing");
            var key = Encoding.UTF8.GetBytes(signingKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParams, out SecurityToken validatedToken);
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message, int? errorCode = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<int?>(false, errorCode, message, null);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}
