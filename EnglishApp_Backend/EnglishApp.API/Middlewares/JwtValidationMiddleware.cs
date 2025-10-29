using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishApp.API.Middlewares
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtValidationMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if endpoint allows anonymous
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await WriteErrorResponse(context, 401, "Missing or invalid Authorization header.");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var signingKey = _config["JWT:SigningKey"];
            var issuer = _config["JWT:Issuer"];
            var audience = _config["JWT:Audience"];

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Disable clock skew to check exact expiration time
                };

                tokenHandler.ValidateToken(token, validationParams, out var validatedToken);
                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                await WriteErrorResponse(context, 401, "Token has expired.");
            }
            catch (SecurityTokenException)
            {
                await WriteErrorResponse(context, 401, "Invalid token.");
            }
            catch (Exception)
            {
                await WriteErrorResponse(context, 401, "Token validation failed.");
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($"{{\"error\": \"{message}\"}}");
        }
    }
}
