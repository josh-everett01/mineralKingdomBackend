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
    }

}

