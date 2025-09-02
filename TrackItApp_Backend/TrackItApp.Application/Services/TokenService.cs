using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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


        #region CreateToken
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                // Add role, IsActive and IsVerified claims
                new Claim("IsVerified", user.IsVerified.ToString()),
                new Claim("role", user.UserType.UserTypeName.ToString().ToLower())
            };


            var Creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // The details of the token
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(15),
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
        public string GenerateRefreshToken()
        {
            //Generate New refresh Token
            const int length = 64;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*,.?";
            var random = new Random();

            var rawChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                rawChars[i] = chars[random.Next(chars.Length)];
            }

            string refreshToken = new string(rawChars);

            return refreshToken;
        }
        #endregion 
    }
}
