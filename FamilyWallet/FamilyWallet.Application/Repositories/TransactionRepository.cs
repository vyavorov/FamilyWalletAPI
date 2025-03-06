using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Domain.Enums;
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
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(FamilyWalletDbContext context) : base(context) { }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByFamilyGroupIdAsync(int familyGroupId)
        {
            return await _context.Transactions.Where(t => t.User.FamilyGroupId == familyGroupId).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type)
        {
            return await _context.Transactions.Where(t => t.Type == type).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
        {
            return await _context.Transactions.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<decimal> GetTotalExpenseAsync()
        {
            return await _context.Transactions.Where(t => t.Type == TransactionType.Expense && t.Amount > 0).SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalIncomeAsync()
        {
            return await _context.Transactions.Where(t => t.Type == TransactionType.Income && t.Amount > 0).SumAsync(t => t.Amount);
        }
    }
}
