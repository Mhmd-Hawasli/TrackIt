using AutoMapper;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Infrastructure.Implementations.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;

namespace EnglishApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class _TestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public _TestController(AppDbContext context, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        #region generate-token
        [HttpGet("generate-token")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GenerateToken()
        {
            var issuer = _config["JWT:Issuer"];
            var audience = _config["JWT:Audience"];
            var signingKey = _config["JWT:SigningKey"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", "user")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                succeeded = true,
                token = tokenString,
                expiresAt = DateTime.UtcNow.AddMinutes(30)
            });
        }
        #endregion

        #region GetLastCode
        [HttpGet("get-last-code")]
        public async Task<IActionResult> GetLastCode()
        {
            var codeModel = await _context.VerificationCodes
                            .AsNoTracking()
                            .Where(c => c.ExpiresAt > DateTime.UtcNow)
                            .OrderByDescending(c => c.ExpiresAt)
                            .Select(c => new
                            {
                                c.Code,
                                c.DeviceId,
                                User = new
                                {
                                    c.UserId,
                                    c.User.Username,
                                    c.User.Email,
                                }
                            })
                            .ToListAsync();
            return Ok(codeModel);
        }
        #endregion


    }
}
