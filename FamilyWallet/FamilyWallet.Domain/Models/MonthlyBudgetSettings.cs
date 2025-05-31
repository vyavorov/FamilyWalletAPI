using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.Models
{
    public class MonthlyBudgetSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1,12)]
        public int Month { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpectedIncome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FixedExpenses { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SavingGoal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CarriedOverAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
