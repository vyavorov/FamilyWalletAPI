using FamilyWallet.Application.Repositories;
using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
using FamilyWallet.Domain.DTOs;
using FamilyWallet.Domain.Models;
using Microsoft.Exchange.WebServices.Data;
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
            decimal desoredSavingGoal = settings?.SavingGoal ?? 0;
            decimal savingGoal = await _transactionRepository.GetSavingsForMonthAsync(userId, year, month);
            decimal carriedOver = settings?.CarriedOverAmount ?? 0;

            var income = await _transactionRepository.GetTotalIncomeForMonthAsync(userId, month, year);
            var expensesUntilYesterday = await _transactionRepository
                .GetTotalExpensesUntilDateInMonthAsync(userId, month, year, today.AddDays(-1));

            decimal spendable = income + carriedOver - savingGoal;
            decimal remaining = spendable - expensesUntilYesterday;

            int totalDays = DateTime.DaysInMonth(year, month);
            int daysLeft = totalDays - today.Day + 1;
            decimal dailyBudget = daysLeft > 0 ? remaining / daysLeft : 0;

            return new DashboardDataDto
            {
                TotalIncome = income,
                TotalExpenses = expensesUntilYesterday,
                DesiredSavingGoal = desoredSavingGoal,
                CarriedOverAmount = carriedOver,
                SpendableAmount = spendable,
                RemainingAmount = remaining,
                DaysLeft = daysLeft,
                DailyBudget = dailyBudget,
                TotalSavings = savingGoal,
            };
        }

        public async Task<MonthlyBudgetSettings> GetCurrentAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _settingsRepository.GetForUserAndMonthAsync(userId, now.Month, now.Year);
        }

        //public async Task SetOrUpdateAsync(MonthlyBudgetSettings settings)
        public async System.Threading.Tasks.Task SetOrUpdateAsync(MonthlyBudgetSettings settings)

        {
            var existing = await _settingsRepository.GetForUserAndMonthAsync(settings.UserId, settings.Month, settings.Year);


            int previousMonth = settings.Month == 1 ? 12 : settings.Month - 1;
            int previousYear = settings.Month == 1 ? settings.Year - 1 : settings.Year;
            var prevMonthSettings = await _settingsRepository.GetForUserAndMonthAsync(settings.UserId, previousMonth, previousYear);

            var incomePrev = await _transactionRepository.GetTotalIncomeForMonthAsync(settings.UserId, previousMonth, previousYear);
            var expensesPrev = await _transactionRepository.GetTotalExpensesForMonthAsync(settings.UserId, previousMonth, previousYear);
            var realSavingsPrev = await _transactionRepository.GetSavingsForMonthAsync(settings.UserId, previousYear, previousMonth);
            var prevCarriedOver = prevMonthSettings.CarriedOverAmount;

            decimal carriedOver = prevCarriedOver + incomePrev - expensesPrev - realSavingsPrev;
            if (carriedOver < 0) carriedOver = 0;

            if (existing == null)
            {
                settings.CarriedOverAmount = carriedOver;
                await _settingsRepository.AddAsync(settings);
            }
            else
            {
                existing.SavingGoal = settings.SavingGoal;
                await _settingsRepository.UpdateAsync(existing);
            }
        }
    }
}