using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyWallet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountsAsync()
        {
            var response = await _accountService.GetAllAccountsAsync();
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountByIdAsync(int accountId)
        {
            var response = await _accountService.GetAccountByIdAsync(accountId);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddAccountAsync([FromBody] AccountDto accountDto)
        {
            var response = await _accountService.CreateAccountAsync(accountDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateAccountAsync([FromBody] AccountDto accountDto)
        {
            var response = await _accountService.UpdateAccountAsync(accountDto);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccountAsync(int accountId)
        {
            var response = await _accountService.DeleteAccountAsync(accountId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAccountsByUserAsync(int userId)
        {
            var response = await _accountService.GetAccountsByUserIdAsync(userId);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }
            return Ok(response.Data);
        }
    }
}
