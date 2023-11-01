using System;
namespace MineralKingdomApi.DTOs.PaymentDTOs
{
    public class PaymentDetailsDto
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MineralId { get; set; }
        public string CheckoutSessionId { get; set; }
    }

}

