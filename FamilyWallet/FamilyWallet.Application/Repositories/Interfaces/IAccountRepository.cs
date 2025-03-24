using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int? id);
        Task<IEnumerable<Account>> GetAllAsync();
        Task AddAsync(Account entity);
        Task UpdateAsync(Account entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsByNameAsync(string name);

        Task<ICollection<Account>> GetAccountsByUserIdAsync(int userId);
        Task<ICollection<Account>> GetAccountsByFamilyIdAsync(int familyGroupId);

    }
}
