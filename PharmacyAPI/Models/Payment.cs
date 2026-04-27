using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyAPI.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        [MaxLength(50)]
        public string? Method { get; set; } // Card, UPI, NetBanking, COD

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;
    }
}
