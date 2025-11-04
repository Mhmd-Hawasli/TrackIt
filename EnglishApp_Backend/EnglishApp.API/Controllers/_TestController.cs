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

        #region GetLastCode
        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                            .AsNoTracking()
                            .Include(u => u.UserType)
                            .Include(u => u.Dictionaries)
                            .Where(u => !u.IsDeleted)
                            .Select(u => new
                            {
                                u.UserId,
                                u.Name,
                                u.Username,
                                u.Email,
                                u.UserTypeId,
                                u.UserType.UserTypeName,
                                Dictionaries = u.Dictionaries.Select(d => new
                                {
                                    d.DictionaryId,
                                    d.DictionaryName
                                })
                            })
                            .ToListAsync();
            return Ok(users);
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
