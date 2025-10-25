using Microsoft.AspNetCore.Mvc;
using EnglishApp.Application.Common;
using EnglishApp.Application.Interfaces.Services;
using EnglishApp.Application.DTOs.DictionaryDto;
using EnglishApp.Application.DTOs.UserDto.User;

namespace EnglishApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;
        public DictionaryController(IDictionaryService dictionaryService) 
        {
            _dictionaryService = dictionaryService;
        }

        #region AddDictionary
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDictionary([FromBody] DictionaryRequest request)
        {
            //get userId from token 
            int userId = int.Parse(HttpContext.Items["UserId"]!.ToString()!);

            var result = await _dictionaryService.AddDictionaryAsync(request, userId);
            if (!result.Succeeded)
            {
                if (result.Message == "User Not Found.")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }
}
