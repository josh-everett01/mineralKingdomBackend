using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MineralKingdomApi.Data.Models;

namespace MineralKingdomApi.Data.Models
{

    public enum PaymentMethodType
    {
        Stripe,
        PayPal
    }

    public enum CheckoutMode
    {
        Payment
    }


    public class CheckoutSessionRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public List<LineItem> LineItems { get; set; } = new List<LineItem>();

        [Required]
        [Url]
        public string SuccessUrl { get; set; }

        [Required]
        [Url]
        public string CancelUrl { get; set; }

        [Required]
        public PaymentMethodType PaymentMethodTypes { get; set; }

        [Required]
        public CheckoutMode Mode { get; set; } = CheckoutMode.Payment;
    }
}
