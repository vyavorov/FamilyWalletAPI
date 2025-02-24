using FamilyWallet.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ServiceResponse> CreateAccountAsync(AccountDto accountDto);
        Task<ServiceResponse<AccountDto>> GetAccountByIdAsync(int accountId);
        Task<ServiceResponse<ICollection<AccountDto>>> GetAllAccountsAsync();
        Task<ServiceResponse<ICollection<AccountDto>>> GetAccountsByUserIdAsync(int userId);
        Task<ServiceResponse<ICollection<AccountDto>>> GetAccountsByFamilyIdAsync(int familyId);
        Task<ServiceResponse> UpdateAccountAsync(AccountDto accountDto);
        Task<ServiceResponse> DeleteAccountAsync(int accountId);
    }
}
