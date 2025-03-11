using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyWallet.Domain.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public int? FamilyGroupId { get; set; }
        [ForeignKey("FamilyGroupId")]
        public FamilyGroup FamilyGroup { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public int UserId { get; set; }
    }
}
