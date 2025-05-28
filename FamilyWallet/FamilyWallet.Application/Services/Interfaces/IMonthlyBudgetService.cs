using FamilyWallet.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface IMonthlyBudgetService
    {
        Task<MonthlyBudgetSettings> GetCurrentAsync(int userId);

        Task SetOrUpdateAsync(MonthlyBudgetSettings settings);
    }
}
