using FamilyWallet.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<MonthlyDashboardDto> GetMonthlyDashboardAsync(int userId, int year, int month);
    }
}
