using FamilyWallet.Domain.Enums;
using FamilyWallet.Domain.Models;
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
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public int? AccountId { get; set; }
        public int? FromAccountId { get; set; }
        public int? ToAccountId { get; set; }
    }
}
