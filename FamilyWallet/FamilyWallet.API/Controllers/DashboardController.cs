using FamilyWallet.Application.Repositories;
using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IDashboardService _dashboardService;
        private readonly IMonthlyBudgetSettingsRepository _monthlyBudgetSettingsRepository;

        public DashboardController(ITransactionRepository transactionRepository, IDashboardService dashboardService, IMonthlyBudgetSettingsRepository monthlyBudgetSettingsRepository)
        {
            _transactionRepository = transactionRepository;
            _dashboardService = dashboardService;
            _monthlyBudgetSettingsRepository = monthlyBudgetSettingsRepository;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardData()
        {
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }
            int userId = int.Parse(userIdClaim.Value);

            var now = DateTime.UtcNow;
            int month = now.Month;
            int year = now.Year;

            var income = await _transactionRepository.GetTotalIncomeForMonthAsync(userId, month, year);
            var expense = await _transactionRepository.GetTotalExpensesForMonthAsync(userId, month, year);
            var savings = await _transactionRepository.

            var settings = await _monthlyBudgetSettingsRepository.GetForUserAndMonthAsync(userId, month, year);
            decimal carriedOver = settings?.CarriedOverAmount ?? 0;
            decimal savingGoal = settings?.SavingGoal ?? 0;

            var totalBalance = income + carriedOver - expense;
            var balanceWithoutSavingGoal = totalBalance - savingGoal;

            return Ok(new { income, expense, balanceWithoutSavingGoal, savingGoal });
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyDashboard([FromQuery] int year, [FromQuery] int month)
        {
            if (year < 2000 || year > DateTime.Now.Year + 1 || month < 1 || month > 12)
            {
                return BadRequest("Invalid year or month.");

            }

            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid or missing user ID");
            }
            var userId = int.Parse(userIdClaim.Value);
            try
            {
                var result = await _dashboardService.GetMonthlyDashboardAsync(userId, year, month);

                if (result == null)
                    return NotFound("No data found for the specified month.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the dashboard data.");
            }
        }
    }
}
