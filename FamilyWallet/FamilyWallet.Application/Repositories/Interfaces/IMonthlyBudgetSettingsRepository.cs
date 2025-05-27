using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Repositories.Interfaces
{
    public interface IMonthlyBudgetSettingsRepository
    {
        Task<MonthlyBudgetSettings> GetForUserAndMonthAsync(int userId, int month, int year);

        Task AddAsync(MonthlyBudgetSettings settings);

        Task UpdateAsync(MonthlyBudgetSettings settings);
    }
}
