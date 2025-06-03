using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<ServiceResponse> AddTransactionAsync(TransactionDto transactionDto);
        Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByUserAsync(int userId);
        Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByFamilyGroupAsync(int familyGroupId);
        Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ServiceResponse<IEnumerable<TransactionDto>>> GetTransactionsByTypeAsync(int userId, TransactionType type);

        Task<ServiceResponse> UpdateTransactionAsync(TransactionDto transactionDto);
        Task<ServiceResponse<TransactionDto>> GetTransactionByIdAsync(int transactionId);

        Task<ServiceResponse> DeleteTransactionAsync(int transactionId);

        Task<ServiceResponse<IEnumerable<TransactionDto>>> GetAllTransactions();

        Task<decimal> GetExpensesForDateAsync(int userId, DateTime date);
        Task<decimal> GetExpensesForMonthAsync(int userId, DateTime date);

        Task<ServiceResponse<decimal>> GetSavingsForMonthAsync(int userId, int year, int month);
    }
}
