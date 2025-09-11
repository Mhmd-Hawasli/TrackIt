using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackItApp.Application.Interfaces.Services;

namespace TrackItApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public IActionResult Get()
        {

            return Ok("ok");
        }


        [HttpGet("AllowAnonymous")]
        [AllowAnonymous]
        public IActionResult GetAllowAnonymous()
        {
            var deviceId = HttpContext.Items["DeviceId"]?.ToString();

            return Ok(deviceId);
        }
    }
}
