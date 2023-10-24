using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Data.Models
{
    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string TransactionId { get; set; }

        [Required]
        [Range(0.01, 1000000)] // Assuming a max limit, adjust as needed
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } // e.g., "card", "paypal"

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // e.g., "succeeded", "pending"

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string CustomerId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

    }

}

