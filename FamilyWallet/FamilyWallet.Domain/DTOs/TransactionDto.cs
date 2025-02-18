using FamilyWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
