using FamilyWallet.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public int? AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

        public int? FromAccountId { get; set; }

        [ForeignKey(nameof(FromAccountId))]
        public Account? FromAccount { get; set; }

        public int? ToAccountId { get; set; }
        [ForeignKey(nameof(ToAccountId))]
        public Account? ToAccount { get; set; }
    }
}
