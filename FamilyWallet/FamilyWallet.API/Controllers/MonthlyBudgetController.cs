using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FamilyWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MonthlyBudgetController : ControllerBase
    {
        private readonly IMonthlyBudgetService _monthlyBudgetService;
        public MonthlyBudgetController(IMonthlyBudgetService monthlyBudgetService)
        {
            _monthlyBudgetService = monthlyBudgetService;
        }
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            int userId = int.Parse(userIdClaim.Value);
            var settings = await _monthlyBudgetService.GetCurrentAsync(userId);
            return Ok(settings);
        }
        [HttpPost("set-or-update")]
        public async Task<IActionResult> SetOrUpdate([FromBody] MonthlyBudgetSettings settings)
        {
            if (settings == null)
            {
                return BadRequest("Settings cannot be null");
            }
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            int userId = int.Parse(userIdClaim.Value);
            settings.UserId = userId;
            await _monthlyBudgetService.SetOrUpdateAsync(settings);
            return NoContent();
        }
    }
}
