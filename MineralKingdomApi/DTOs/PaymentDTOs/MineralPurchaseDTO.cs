using System;
namespace MineralKingdomApi.DTOs.PaymentDTOs
{
    public class MineralPurchaseDto
    {
        public int UserId { get; set; }
        public int MineralId { get; set; }
        public string? PaymentMethodId { get; set; }
        public string? StripeCustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "usd"; // Default to USD, or set based on your application's needs
        public string? DiscountCode { get; set; }
        public string? Notes { get; set; }
        public string? OrderId { get; set; }
    }

}

