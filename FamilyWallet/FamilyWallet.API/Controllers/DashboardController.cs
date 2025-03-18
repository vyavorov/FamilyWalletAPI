using FamilyWallet.Application.Repositories.Interfaces;
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

        public DashboardController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }
            int userId = int.Parse(userIdClaim.Value);
            var income = await _transactionRepository.GetTotalIncomeAsync(userId);
            var expense = await _transactionRepository.GetTotalExpenseAsync(userId);
            var totalBalance = income - expense;

            return Ok(new { income, expense, totalBalance });
        }
    }
}
