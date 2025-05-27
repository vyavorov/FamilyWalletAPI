using FamilyWallet.Application.Repositories.Interfaces;
using FamilyWallet.Application.Services.Interfaces;
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
        private readonly IMonthlyBudgetSettingsRepository _repository;
        public MonthlyBudgetService(IMonthlyBudgetSettingsRepository repository)
        {
            _repository = repository;
        }

        public async Task<MonthlyBudgetSettings> GetCurrentAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _repository.GetForUserAndMonthAsync(userId, now.Month, now.Year);
        }

        public async Task SetOrUpdateAsync(MonthlyBudgetSettings settings)
        {
            var existing = await _repository.GetForUserAndMonthAsync(settings.UserId, settings.Month, settings.Year);
            if (existing == null)
            {
                await _repository.AddAsync(settings);
            }
            else
            {
                existing.ExpectedIncome = settings.ExpectedIncome;
                existing.FixedExpenses = settings.FixedExpenses;
                existing.SavingGoal = settings.SavingGoal;
                await _repository.UpdateAsync(existing);
            }
        }
    }
}