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

        // Bid Types
        public decimal? MaximumBid { get; set; } // For Proxy Bidding
        public bool IsDelayedBid { get; set; } = false; // For Delayed Bid
        public DateTime? ActivationTime { get; set; } // For Delayed Bid
        public string? BidType { get; set; } // "AuctionBid", "ProxyBid", "DelayedBid"

        // Add RowVersion property for concurrency control
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
