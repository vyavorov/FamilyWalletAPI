using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FamilyWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Adding new transaction
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddTransactionAsync([FromBody] TransactionDto transactionDto)
        {
            var response = await _transactionService.AddTransactionAsync(transactionDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }


        /// <summary>
        /// Връща всички транзакции на семейната група
        /// </summary>
        [HttpGet("familyGroup/{familyGroupId}")]
        public async Task<IActionResult> GetTransactionsByFamilyGroup(int familyGroupId)
        {
            var result = await _transactionService.GetTransactionsByFamilyGroupAsync(familyGroupId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }


        /// <summary>
        /// Getting all transaction for specific user
        /// </summary>
        [HttpGet("user")]
        public async Task<IActionResult> GetTransactionsByUserAsync()
        {
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            int userId = int.Parse(userIdClaim.Value);

            var response = await _transactionService.GetTransactionsByUserAsync(userId);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        /// <summary>
        /// Getting all transactions by given date range
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetTransactionsByDateRangeAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var response = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }


        /// <summary>
        /// Getting all income/expenses for specific user
        /// </summary>
        [HttpGet("user/{userId}/type/{type}")]
        public async Task<IActionResult> GetTransactionsByTypeAsync(int userId, TransactionType type)
        {
            var response = await _transactionService.GetTransactionsByTypeAsync(userId, type);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        /// <summary>
        /// Updating transaction
        /// </summary>
        [HttpPut("{transactionId}")]
        public async Task<IActionResult> UpdateTransactionAsync([FromBody] TransactionDto transactionDto)
        {
            var response = await _transactionService.UpdateTransactionAsync(transactionDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        /// <summary>
        /// Getting transaction by Id
        /// </summary>
        /// 
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionByIdAsync(int transactionId)
        {
            var response = await _transactionService.GetTransactionByIdAsync(transactionId);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }


        /// <summary>
        /// Deleting transaction by id
        /// </summary>
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeleteTransactionAsync(int transactionId)
        {
            var response = await _transactionService.DeleteTransactionAsync(transactionId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        /// <summary>
        /// Getting all transactions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTransactions()
        {
            var response = await _transactionService.GetAllTransactions();
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetExpensesForDate([FromQuery] DateTime date)
        {
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            int userId = int.Parse(userIdClaim.Value);
            var total = await _transactionService.GetExpensesForDateAsync(userId, date);
            return Ok(new { total });
        }

        [HttpGet("spent-so-far")]
        public async Task<IActionResult> GetSpentSoFarThisMonth()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);
            var total = await _transactionService.GetExpensesForMonthAsync(userId, DateTime.UtcNow);
            return Ok(new { total });
        }

        [HttpGet("savings")]
        public async Task<IActionResult> GetSavings()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            { 
                return Unauthorized(); 
            }

            int userId = int.Parse(userIdClaim.Value);

            var now = DateTime.UtcNow;
            var result = await _transactionService.GetSavingsForMonthAsync(userId, now.Year, now.Month);

            if (!result.Success)
            {
                return StatusCode(500, result.Message);
            }

            return Ok(result.Data);
        }

    }
}
