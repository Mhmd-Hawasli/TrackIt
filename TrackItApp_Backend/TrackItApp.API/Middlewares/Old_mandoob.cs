using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TrackItApp.Application.Common;
using TrackItApp.Application.Interfaces;
using TrackItApp.Application.Interfaces.Services;

namespace MandoobApp.API.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration; // 1. Add IConfiguration field

        // 2. Inject IConfiguration into the constructor
        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork _unitOfWork, IAuthService authService)
        {
            // Step 1: Check the endpoint for the [AllowAnonymous] attribute.
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // Step 2: Check for the 'Device-Id' header, which is required for authenticated requests.
            var currentDeviceId = context.Request.Headers["Device-Id"].FirstOrDefault()?.ToLower();
            if (currentDeviceId == null)
            {
                await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "Request header 'Device-Id' is missing.");
                return;
            }

            // Step 3: Check for the access token and validate it.
            var accessToken = context.Request.Cookies["AccessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var ValidIssuer = _configuration["JWT:Audience"];
                    // 3. Get the key from appsettings.json using the injected IConfiguration
                    var jwtSigningKey = _configuration["JWT:SigningKey"];
                    var signingKeyBytes = Encoding.UTF8.GetBytes(jwtSigningKey);

                    // Attempt to validate the token. This will throw an exception if the token is invalid or expired.
                    var principal = tokenHandler.ValidateToken(
                         accessToken,
                         new TokenValidationParameters
                         {
                             ValidateIssuerSigningKey = true,
                             IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
                             ValidateIssuer = true,
                             ValidIssuer = _configuration["JWT:Issuer"],
                             ValidateAudience = false,
                             ValidAudience = _configuration["JWT:Audience"],
                             ClockSkew = TimeSpan.Zero,
                             ValidateLifetime = true
                         },
                         out SecurityToken validatedToken);


                    // If validation succeeds, set the user's principal and continue.
                    context.User = principal;
                    await _next(context);
                    return;
                }
                catch (SecurityTokenExpiredException)
                {
                    // The token has expired; call the refresh token method.
                    //var updateResult = await authService
                    //if (updateResult.Succeeded)
                    //{
                    //    // If the refresh succeeded, continue to the next middleware.
                    //    await _next(context);
                    //    return;
                    //}
                    //else
                    //{
                    //    // If the refresh failed, return an unauthorized error.
                    //    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, updateResult.Message ?? "Failed to refresh token.");
                    //    return;
                    //}
                }
                catch (Exception)
                {
                    // If validation fails for any other reason (e.g., invalid signature),
                    // return an unauthorized error.
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Invalid token.");
                    return;
                }
            }

            // Step 4: If no access token is found, or if validation fails, return an unauthorized error.
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "No access token provided.");
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message, int? data = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new ApiResponse<int?>(false, data, message, null);
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}