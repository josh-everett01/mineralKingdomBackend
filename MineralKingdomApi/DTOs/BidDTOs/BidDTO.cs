using System;
namespace MineralKingdomApi.DTOs.BidDTOs
{
    public class BidDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public int UserId { get; set; }
        public int AuctionId { get; set; }

        public decimal? MaximumBid { get; set; }
        public bool IsDelayedBid { get; set; }
        public DateTime? ActivationTime { get; set; }
        public string? BidType { get; set; }

        // Include the AuctionXmin property for optimistic concurrency
        public uint AuctionXmin { get; set; }
    }

}

