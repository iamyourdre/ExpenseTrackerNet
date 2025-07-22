using ExpenseTrackerNet.Server.Services;
using ExpenseTrackerNet.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTrackerNet.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryWriteDTO request)
        {
            if (request == null)
            {
                return BadRequest("Category data is required.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { Errors = errors });
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            request.UserId = userId;
            var result = await _categoryService.CreateCategoryAsync(request);
            if (result == null)
            {
                return BadRequest("Failed to create category.");
            }
            return Ok(result);
        }


        [HttpPatch("update")]
        public async Task<IActionResult> UpdateCategoryAsync([FromBody] CategoryUpdateDTO request)
        {
            if (request == null || request.Id == Guid.Empty)
            {
                return BadRequest("Category data with a valid Id is required.");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { Errors = errors });
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            var result = await _categoryService.UpdateCategoryAsync(userId, request);
            if (result == null)
            {
                return NotFound("Category not found or update failed.");
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Category Id is required.");
            }
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null)
            {
                return NotFound("Category not found.");
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCategoriesAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            var result = await _categoryService.GetUserCategoriesAsync(userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Category Id is required.");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("Session is missing or invalid.");
            }
            var result = await _categoryService.DeleteCategoryAsync(userId, id);
            if (!result)
            {
                return NotFound("Category not found or deletion failed.");
            }
            return NoContent();

        }
    }
}
