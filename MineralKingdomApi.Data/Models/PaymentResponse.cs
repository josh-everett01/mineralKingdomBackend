using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Data.Models
{
    public class PaymentResponse
    {
        // Primary key for the entity.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // A unique ID for the transaction. This should be required as every transaction should have an ID.
        [Required]
        [MaxLength(100)] // Limit the length based on expected transaction ID length.
        public string TransactionId { get; set; }

        // The status of the payment. This should be required to always inform the status of the payment.
        [Required]
        [MaxLength(50)] // Limit the length for efficiency and data integrity.
        public string Status { get; set; }

        // A message, especially useful in case of errors or additional info. This can be nullable.
        [MaxLength(500)] // Limit the length based on expected message length.
        public string? Message { get; set; }

        // The amount that was charged. This should be required.
        [Required]
        public decimal Amount { get; set; }

        // The currency of the payment. Defaulting to USD, but can be adjusted. This should also be required.
        [Required]
        [MaxLength(10)] // Limit the length based on typical currency code lengths (e.g., "USD").
        public string Currency { get; set; }

        // Collection navigation property for metadata.
        public ICollection<PaymentResponseMetadata> Metadata { get; set; } = new List<PaymentResponseMetadata>();
    }
}
