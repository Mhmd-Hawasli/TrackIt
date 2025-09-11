using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrackItApp.Application.Interfaces.Services;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Services
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

        #region ValidateExpiredAccessToken
        public ClaimsPrincipal? ValidateExpiredAccessToken(string accessToken)
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

            try
            {
                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken validatedToken);

                // ✅ signature صالح
                return principal;
            }
            catch (SecurityTokenException)
            {
                // التوكن غير صالح (signature خطأ)
                return null;
            }
        }
        #endregion

        #region CreateToken
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim("role", user.UserType.UserTypeName.ToString().ToLower())

            };


            var Creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // The details of the token
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = Creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var Token = TokenHandler.CreateToken(TokenDescriptor);
            return TokenHandler.WriteToken(Token);
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
