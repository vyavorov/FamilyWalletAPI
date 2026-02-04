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

            // Реални приходи и разходи
            var income = await _transactionRepository.GetTotalIncomeForMonthAsync(userId, month, year);
            var expense = await _transactionRepository.GetTotalExpensesForMonthAsync(userId, month, year);

            // Реално спестено: сума по сметка "Спестявания" за текущия месец
            var realSavings = await _transactionRepository.GetSavingsForMonthAsync(userId, year, month);

            // Настройки за месеца
            var settings = await _monthlyBudgetSettingsRepository.GetForUserAndMonthAsync(userId, month, year);
            decimal desiredSavingGoal = settings?.SavingGoal ?? 0;

            // Изчисляваме carriedOver винаги динамично на база миналия месец
            int prevMonth = month == 1 ? 12 : month - 1;
            int prevYear = month == 1 ? year - 1 : year;

            //Настройки за предния месец
            var prevMonthSettings = await _monthlyBudgetSettingsRepository.GetForUserAndMonthAsync(userId, prevMonth, prevYear);
            var prevCarriedOver = prevMonthSettings.CarriedOverAmount;


            var prevIncome = await _transactionRepository.GetTotalIncomeForMonthAsync(userId, prevMonth, prevYear);
            var prevExpenses = await _transactionRepository.GetTotalExpensesForMonthAsync(userId, prevMonth, prevYear);
            var prevSavings = await _transactionRepository.GetSavingsForMonthAsync(userId, prevYear, prevMonth);

            decimal carriedOver = prevIncome + prevCarriedOver - prevExpenses - prevSavings;
            if (carriedOver < 0) carriedOver = 0;
            if (prevMonth == 1 && prevYear == 2026) carriedOver = 0; // Специален случай за февруари 2026, когато приложението стартира

            // Калкулация на баланса
            decimal totalBalance = income + carriedOver - expense;
            decimal balanceWithoutSavings = totalBalance - realSavings;

            return Ok(new
            {
                income,
                expense,
                carriedOver,
                balanceWithoutSavingGoal = balanceWithoutSavings,
                savingGoal = desiredSavingGoal,
                realSavings
            });
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
