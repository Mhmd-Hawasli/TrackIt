using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.UserDto.Auth;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }



        #region CreateToken
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("role", user.UserType.UserTypeName.ToString().ToLower())

            };


            var Creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = Creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token); // هذا الـ string اللي ترسله للـ client



            return tokenString;
        }
        #endregion

        #region ValidateExpiredAccessToken
        public int ValidateExpiredAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,       // ⚠️ تجاهل انتهاء الصلاحية
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key
                };


                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);
                var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (principal == null || string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new ArgumentException("Your access token is invalid. Please log in again.");
                }
                // ✅ signature صالح
                return userId;
            }
            catch (SecurityTokenException)
            {
                throw;
            }
        }
        #endregion

        #region GenerateRefreshToken
        public (string refreshToken, string hashedRefreshToken) GenerateRefreshToken()
        {
            // generate random Refresh Token 
            const int length = 72;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();

            var rawChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                rawChars[i] = chars[random.Next(chars.Length)];
            }

            string refreshToken = new string(rawChars);

            // Hash with BCrypt
            string hashedRefreshToken = BCrypt.Net.BCrypt.HashPassword(refreshToken);

            // return before and after hashed
            return (refreshToken, hashedRefreshToken);
        }
        #endregion 
    }
}
