using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.PaymentDTOs
{
    public class CheckoutRequestDto
    {
        [Required]
        public List<LineItemDto> LineItems { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class LineItemDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int MineralId { get; set; }
    }
}

