using FamilyWallet.Domain.Enums;
using FamilyWallet.Domain.Models;
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

    }
}
