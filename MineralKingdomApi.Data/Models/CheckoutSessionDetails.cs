using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MineralKingdomApi.Data.Models;

namespace MineralKingdomApi.Data.Models
{

    public enum CheckoutSessionStatus
    {
        Pending,
        Completed,
        Cancelled,
        Failed
    }

    public class CheckoutSessionDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SessionId { get; set; }

        [Required]
        [MaxLength(50)]
        public CheckoutSessionStatus Status { get; set; }

        [Required]
        public List<LineItem> LineItems { get; set; } = new List<LineItem>();

        [MaxLength(100)]
        public string CustomerId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

    }

}

