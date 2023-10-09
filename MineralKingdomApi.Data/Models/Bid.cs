using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{
    public class Bid
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime BidTime { get; set; }

        // Relationship Navigation Property: A Bid is associated with one User
        public int UserId { get; set; }
        public User? User { get; set; }

        // Relationship Navigation Property: A Bid is associated with one Auction
        public int AuctionId { get; set; }
        public Auction? Auction { get; set; }
    }
}
