using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITransactionRepository _transactionRepository;

        public DashboardService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<MonthlyDashboardDto> GetMonthlyDashboardAsync(int userId, int year, int month)
        {
            if (year < 2000 || year > DateTime.Now.Year + 1 || month < 1 || month > 12)
            {
                throw new ArgumentException("Invalid year or month");
            }

            var transactions = await _transactionRepository.GetTransactionsForMonthAsync(userId, year, month);

            if (transactions == null || !transactions.Any())
            {
                return new MonthlyDashboardDto();
            }

            var dto = new MonthlyDashboardDto
            {
                Income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                ByCategory = transactions
                    .Where(t => t.Type == TransactionType.Expense && t.Category != null)
                    .GroupBy(t => t.Category.Name)
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)),
                DailyBreakdown = transactions
                    .GroupBy(t => t.Date.Date)
                    .Select(g => new DailyBreakdownDto
                    {
                        Date = g.Key,
                        Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                        Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                    })
                    .OrderBy(x => x.Date)
                    .ToList(),
                Transactions = transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Type = t.Type,
                    CategoryId = t.Category.Id,
                    Date = t.Date,
                    Description = t.Description
                }).ToList()
            };

            return dto;

        }
    }
}
