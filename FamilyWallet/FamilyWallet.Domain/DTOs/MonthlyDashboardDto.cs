using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.DTOs
{
    public class MonthlyDashboardDto
    {
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }

        public decimal Balance => Income - Expenses;

        public Dictionary<string, decimal> ByCategory { get; set; }

        public List<DailyBreakdownDto> DailyBreakdown { get; set; }

        public List<TransactionDto> Transactions { get; set; }
    }

    public class DailyBreakdownDto
    {
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }

        public DateTime Date { get; set; }
    }
}
