using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrackItApp.Domain.Entities;

namespace TrackItApp.Application.Interfaces.Services
{
    public interface ITokenService
    {
        ClaimsPrincipal? ValidateExpiredAccessToken(string accessToken);
        string CreateToken(User user);
        (string refreshToken, string hashedRefreshToken) GenerateRefreshToken();
    }
}
