using EnglishApp.Application.Common;
using EnglishApp.Application.DTOs.DictionaryDto;
using EnglishApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishApp.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user dictionaries,
    /// including creation, update, deletion, and retrieval operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]

    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;

        public DictionaryController(IDictionaryService dictionaryService)
        {
            _dictionaryService = dictionaryService;
        }

        #region AddDictionary
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<DictionaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDictionary([FromBody] DictionaryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(ModelState));

            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var result = await _dictionaryService.AddDictionaryAsync(request, userId);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion

        #region UpdateDictionary
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<DictionaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDictionary([FromBody] DictionaryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>(ModelState));

            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var result = await _dictionaryService.UpdateDictionaryAsync(request, userId);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion

        #region DeleteDictionary
        [HttpDelete("{dictionaryId:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDictionary(int dictionaryId)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var result = await _dictionaryService.DeleteDictionaryAsync(dictionaryId, userId);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }
        #endregion

        #region GetDictionaryById
        [HttpGet("{dictionaryId:int}")]
        [ProducesResponseType(typeof(ApiResponse<DictionaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<DictionaryResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDictionaryById(int dictionaryId)
        {
            var result = await _dictionaryService.GetDictionaryByIdAsync(dictionaryId);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }
        #endregion

        #region GetAllDictionariesByUser
        [HttpGet("user")]
        [ProducesResponseType(typeof(ApiResponse<ICollection<DictionaryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllDictionariesByUser()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            var result = await _dictionaryService.GetAllDictionariesByUserAsync(userId);

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }
        #endregion
    }
}
