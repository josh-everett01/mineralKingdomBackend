using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Data.Models
{
    public class PaymentRequest
    {
        // Primary key for the entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // The amount to be charged. This should always be required.
        [Required]
        public decimal Amount { get; set; }

        // The currency for the payment. Defaulting to USD, but can be adjusted. This should also be required.
        [Required]
        public string Currency { get; set; } = "USD";

        // The payment method, e.g., "PayPal" or "Stripe". This should be required to determine the payment gateway.
        [Required]
        [MaxLength(50)] // Limit the length for efficiency and data integrity.
        public string PaymentMethod { get; set; }

        // A description of the payment. This can be nullable as it's optional.
        [MaxLength(500)] // Limit the length based on expected description length.
        public string? Description { get; set; }

        // An optional reference or order ID from your system. This can be nullable.
        [MaxLength(100)] // Limit the length based on expected order ID length.
        public string? OrderId { get; set; }

        // Collection navigation property
        public ICollection<PaymentRequestMetadata> Metadata { get; set; } = new List<PaymentRequestMetadata>();
    }
}
