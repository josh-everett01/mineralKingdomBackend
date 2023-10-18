using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Data.Models
{
    public class PaymentResponseMetadata
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary key

        // Foreign key to the PaymentResponse
        [Required]
        public int PaymentResponseId { get; set; }

        [ForeignKey("PaymentResponseId")]
        public PaymentResponse PaymentResponse { get; set; } // Navigation property

        [Required]
        [MaxLength(100)] // Limit the length based on expected key length.
        public string Key { get; set; } // The metadata key

        [Required]
        [MaxLength(500)] // Limit the length based on expected value length.
        public string Value { get; set; } // The metadata value
    }
}
