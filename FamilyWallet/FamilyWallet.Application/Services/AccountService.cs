using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        public AccountService(IAccountRepository accountRepository, IUserRepository userRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse> CreateAccountAsync(AccountDto accountDto)
        {
            var user = await _userRepository.GetByIdAsync(accountDto.OwnerId);
            if (user == null)
            {
                return new ServiceResponse { Message = "User not found", Success = false };
            }
            var account = new Account
            {
                Name = accountDto.Name,
                UserId = accountDto.OwnerId,
                Balance = accountDto.Balance,
                FamilyGroupId = accountDto.FamilyGroupId,
            };
            await _accountRepository.AddAsync(account);
            return new ServiceResponse { Success = true, Message="Account added successfuly"};
        }

        public async Task<ServiceResponse> DeleteAccountAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            await _accountRepository.DeleteAsync(accountId);
            return new ServiceResponse { Success = true, Message = "Account deleted successfuly" };
        }

        public async Task<ServiceResponse<AccountDto>> GetAccountByIdAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return new ServiceResponse<AccountDto> { Message = "Account not found" };
            }
            var accountDto = new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                OwnerId = account.UserId,
                Balance = account.Balance,
                FamilyGroupId = account.FamilyGroupId,
            };
            return new ServiceResponse<AccountDto> { Success = true, Data = accountDto };
        }

        public async Task<ServiceResponse<ICollection<AccountDto>>> GetAccountsByFamilyIdAsync(int familyId)
        {
            var accounts = await _accountRepository.GetAccountsByFamilyIdAsync(familyId);
            if (!accounts.Any())
            {
                return new ServiceResponse<ICollection<AccountDto>> { Message = "No accounts found" , Success = false};
            }
            var accountsDtos = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                OwnerId = a.UserId,
                Balance = a.Balance,
                FamilyGroupId = a.FamilyGroupId,
            }).ToList();
            return new ServiceResponse<ICollection<AccountDto>> { Success = true, Data = accountsDtos };
        }

        public async Task<ServiceResponse<ICollection<AccountDto>>> GetAccountsByUserIdAsync(int userId)
        {
            var accounts = await _accountRepository.GetAccountsByUserIdAsync(userId);
            if (!accounts.Any())
            {
                return new ServiceResponse<ICollection<AccountDto>> { Message = "No accounts found", Success = true, Data = new List<AccountDto>() };
            }
            var accountsDtos = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                OwnerId = a.UserId,
                Balance = a.Balance,
                FamilyGroupId = a.FamilyGroupId,
            }).ToList();
            return new ServiceResponse<ICollection<AccountDto>> { Success = true, Data = accountsDtos };
        }

        public async Task<ServiceResponse<ICollection<AccountDto>>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            if (!accounts.Any())
            {
                return new ServiceResponse<ICollection<AccountDto>> { Message = "No accounts found" };
            }
            var accountsDtos = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                OwnerId = a.UserId,
                Balance = a.Balance,
                FamilyGroupId = a.FamilyGroupId,
            }).ToList();
            return new ServiceResponse<ICollection<AccountDto>> { Success = true, Data = accountsDtos };
        }

        public async Task<ServiceResponse> UpdateAccountAsync(AccountDto accountDto)
        {
            var account = await _accountRepository.GetByIdAsync(accountDto.Id);
            if (account == null)
            {
                return new ServiceResponse { Message = "Account not found", Success = false };
            }
            account.Name = accountDto.Name;
            account.Balance = accountDto.Balance;
            account.FamilyGroupId = accountDto.FamilyGroupId;
            await _accountRepository.UpdateAsync(account);
            return new ServiceResponse { Success = true, Message = "Account updated successfuly" };
        }
    }
}
