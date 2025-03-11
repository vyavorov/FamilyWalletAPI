using FamilyWallet.Application.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var income = await _transactionRepository.GetTotalIncomeAsync();
            var expense = await _transactionRepository.GetTotalExpenseAsync();
            var totalBalance = income - expense;

            return Ok(new { income, expense, totalBalance });
        }
    }
}
