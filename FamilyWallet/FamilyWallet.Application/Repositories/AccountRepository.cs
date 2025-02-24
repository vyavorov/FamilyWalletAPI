using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Domain.Models;
using FamilyWallet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(FamilyWalletDbContext context) : base(context){ }

        public async Task<bool> ExistsByNameAsync(string name) => await _context.Accounts.AnyAsync(a => a.Name == name);

        public async Task<ICollection<Account>> GetAccountsByUserIdAsync(int userId) => await _context.Accounts.Where(a => a.UserId == userId).ToListAsync();

        public async Task<ICollection<Account>> GetAccountsByFamilyIdAsync(int familyGroupId) => await _context.Accounts.Where(a => a.User.FamilyGroupId == familyGroupId).ToListAsync();

    }
}
