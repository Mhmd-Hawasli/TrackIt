using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EnglishApp.Domain.Entities;

namespace EnglishApp.Application.Interfaces.Services
{
    public interface ITokenService
    {
        int ValidateExpiredAccessToken(string accessToken);
        string CreateToken(User user);
        (string refreshToken, string hashedRefreshToken) GenerateRefreshToken();
    }
}
