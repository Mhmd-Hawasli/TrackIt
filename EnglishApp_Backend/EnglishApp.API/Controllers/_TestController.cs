using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using EnglishApp.Application.Interfaces;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Infrastructure.Implementations.Persistence;

namespace EnglishApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _TestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public _TestController(AppDbContext context, IUnitOfWork unitOfWork,IMapper mapper)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region GetLastCode
        [HttpGet("get-last-code")]
        public async Task<IActionResult> GetLastCode()
        {
            var codeModel = await _context.VerificationCodes
                            .AsNoTracking()
                            .Where(c => c.ExpiresAt > DateTime.UtcNow)
                            .OrderByDescending(c => c.ExpiresAt)
                            .Select(c => new {
                                c.Code,
                                c.DeviceId,
                                User = new { 
                                    c.UserId,
                                    c.User.Username,
                                    c.User.Email,
                                }
                            })
                            .ToListAsync();
            return Ok("codeModel");
        } 
        #endregion

        
    }
}
