using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var response = await _categoryService.GetAllCategories();
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetCategoriesByUserAsync()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            int userId = int.Parse(userIdClaim.Value);

            var response = await _categoryService.GetCategoriesByUserAsync(userId);

            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }


        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int categoryId)
        {
            var response = await _categoryService.GetCategoryByIdAsync(categoryId);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync([FromBody] CategoryDto categoryDto)
        {
            var response = await _categoryService.AddCategoryAsync(categoryDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }
    }
}
