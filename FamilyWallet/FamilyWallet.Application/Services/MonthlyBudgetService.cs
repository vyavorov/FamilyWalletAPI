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
    public class MonthlyBudgetService : IMonthlyBudgetService
    {
        private readonly IMonthlyBudgetSettingsRepository _settingsRepository;
        private readonly ITransactionRepository _transactionRepository;
        public MonthlyBudgetService(IMonthlyBudgetSettingsRepository repository, ITransactionRepository transactionRepository)
        {
            _settingsRepository = repository;
            _transactionRepository = transactionRepository;
        }

        public async Task<DashboardDataDto> GetBudgetOverviewAsync(int userId)
        {
            var now = DateTime.UtcNow;
            int month = now.Month;
            int year = now.Year;
            var today = now.Date;

            var settings = await _settingsRepository.GetForUserAndMonthAsync(userId, month, year);
            decimal savingGoal = settings?.SavingGoal ?? 0;

            var income = await _transactionRepository.GetTotalIncomeForMonthAsync(userId, month, year);

            var expensesUntilYesterday = await _transactionRepository.GetTotalExpensesUntilDateAsync(userId, today.AddDays(-1));

            decimal spendable = income - savingGoal;
            decimal remaining = spendable - expensesUntilYesterday;

            int totalDays = DateTime.DaysInMonth(year, month);
            int daysLeft = totalDays - today.Day + 1;

            decimal dailyBudget = daysLeft > 0 ? remaining / daysLeft : 0;

            return new DashboardDataDto
            {
                TotalIncome = income,
                TotalExpenses = expensesUntilYesterday,
                SavingGoal = savingGoal,
                SpendableAmount = spendable,
                RemainingAmount = remaining,
                DaysLeft = daysLeft,
                DailyBudget = dailyBudget
            };
        }


        public async Task<MonthlyBudgetSettings> GetCurrentAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _settingsRepository.GetForUserAndMonthAsync(userId, now.Month, now.Year);
        }

        public async Task SetOrUpdateAsync(MonthlyBudgetSettings settings)
        {
            var existing = await _settingsRepository.GetForUserAndMonthAsync(settings.UserId, settings.Month, settings.Year);
            if (existing == null)
            {
                await _settingsRepository.AddAsync(settings);
            }
            else
            {
                existing.ExpectedIncome = settings.ExpectedIncome;
                existing.FixedExpenses = settings.FixedExpenses;
                existing.SavingGoal = settings.SavingGoal;
                await _settingsRepository.UpdateAsync(existing);
            }
        }


    }
}