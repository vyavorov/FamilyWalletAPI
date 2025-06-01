using FamilyWallet.Domain.Enums;
using FamilyWallet.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Transaction>> GetByFamilyGroupIdAsync(int familyGroupId);
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type);
        Task<decimal> GetTotalIncomeAsync(int userId);
        Task<decimal> GetTotalExpenseAsync(int userId);
        Task<IEnumerable<Transaction>> GetAllOrderedTransactions();
        Task<List<Transaction>> GetTransactionsForMonthAsync(int userId, int year, int month);

        Task<decimal> GetExpensesTotalForDateAsync(int userId, DateTime date);
        Task<decimal> GetExpensesTotalForMonthAsync(int userId, DateTime date);
        Task<decimal> GetTotalIncomeForMonthAsync(int userId, int month, int year);
        Task<decimal> GetTotalExpensesForMonthAsync(int userId, int month, int year);
        public Task<decimal> GetTotalExpensesUntilDateAsync(int userId, DateTime date);

        Task<decimal> GetTotalExpensesUntilDateInMonthAsync(int userId, int month, int year, DateTime date);


    }
}
