using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        public int? FamilyGroupId { get; set; }

        [ForeignKey(nameof(FamilyGroupId))]
        public FamilyGroup? FamilyGroup { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Balance { get; set; } = 0;
    }
}
