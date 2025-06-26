using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinexaApi.Models
{
    [Table("WalletItem")]
    public class WalletItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Precision(18, 8)]
        public decimal Quantity { get; set; }

        [Precision(18, 8)]
        public decimal PurchasePrice { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;
    }
}
