using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Models
{
    public class CustomerInquiry
    {
        [Key]
        public int InquiryId { get; set; }

        // Assuming CustomerId is the Id from the User model
        [ForeignKey("User")]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Default to "Pending"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
