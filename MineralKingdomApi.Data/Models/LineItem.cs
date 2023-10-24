using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Data.Models
{
    public class LineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary key for the LineItem

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1000)] // Assuming a max quantity limit, adjust as needed
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 1000000)] // Assuming a max price limit, adjust as needed
        public decimal Price { get; set; }

        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }

        public int? MineralId { get; set; }
        public virtual Mineral Mineral { get; set; }

        // Required Foreign key for CheckoutSessionDetails
        [Required]
        public int CheckoutSessionDetailsId { get; set; }
        [ForeignKey("CheckoutSessionDetailsId")]
        public virtual CheckoutSessionDetails CheckoutSessionDetails { get; set; }

    }
}


