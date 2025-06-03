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

        public async Task<IEnumerable<Transaction>> GetAllOrderedTransactions()
        {
            var transactions = await _context.Transactions.OrderByDescending(t => t.Date).ToListAsync();
            return transactions;
        }

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
            return await _context.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).ToListAsync();
        }

        public async Task<decimal> GetExpensesTotalForDateAsync(int userId, DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Type == TransactionType.Expense &&
                    t.Date.Date == date.Date)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;
        }

        public async Task<decimal> GetExpensesTotalForMonthAsync(int userId, DateTime date)
        {
            date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Type == TransactionType.Expense &&
                    t.Date.Month == date.Month)
                .SumAsync(t => (decimal?)t.Amount) ?? 0m;
        }

        public async Task<decimal> GetTotalExpenseAsync(int userId)
        {
            return await _context.Transactions.Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.Amount > 0).SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpensesForMonthAsync(int userId, int month, int year)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Expense)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<decimal> GetTotalExpensesUntilDateAsync(int userId, DateTime date)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense && t.Date.Date <= date.Date)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<decimal> GetTotalIncomeAsync(int userId)
        {
            return await _context.Transactions.Where(t => t.UserId == userId && t.Type == TransactionType.Income && t.Amount > 0).SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalIncomeForMonthAsync(int userId, int month, int year)
        {
            return await _context.Transactions.Where(t => t.UserId == userId && t.Type == TransactionType.Income && t.Date.Month == month && t.Date.Year == year).SumAsync(t => t.Amount);
        }

        public async Task<List<Transaction>> GetTransactionsForMonthAsync(int userId, int year, int month)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date.Year == year && t.Date.Month == month)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalExpensesUntilDateInMonthAsync(int userId, int month, int year, DateTime date)
        {
            return await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Type == TransactionType.Expense &&
                    t.Date.Year == year &&
                    t.Date.Month == month &&
                    t.Date.Date <= date.Date
                )
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<decimal> GetSavings(int userId)
        {
            return await _context.Transactions.Where(t => t.UserId == userId && t.Account!.Name.ToLower() == "спестявания")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }
    }
}
