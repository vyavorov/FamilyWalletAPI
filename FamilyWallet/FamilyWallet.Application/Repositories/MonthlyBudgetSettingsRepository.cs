using FamilyWallet.Application.Repositories.Interfaces;
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
    public class MonthlyBudgetSettingsRepository : IMonthlyBudgetSettingsRepository
    {
        private readonly FamilyWalletDbContext _context;
        public MonthlyBudgetSettingsRepository(FamilyWalletDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MonthlyBudgetSettings settings)
        {
            _context.MonthlyBudgetSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        public async Task<MonthlyBudgetSettings> GetForUserAndMonthAsync(int userId, int month, int year)
        {
            return await _context.MonthlyBudgetSettings.FirstOrDefaultAsync(x => x.UserId == userId && x.Year == year && x.Month == month);
        }

        public async Task UpdateAsync(MonthlyBudgetSettings settings)
        {
            _context.MonthlyBudgetSettings.Update(settings);
            await _context.SaveChangesAsync();
        }
    }
}
