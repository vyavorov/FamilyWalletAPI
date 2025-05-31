using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.DTOs
{
    public class DashboardDataDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal SavingGoal { get; set; }
        public decimal SpendableAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal DailyBudget { get; set; }
        public int DaysLeft { get; set; }
        public decimal CarriedOverAmount { get; set; }
    }
}
