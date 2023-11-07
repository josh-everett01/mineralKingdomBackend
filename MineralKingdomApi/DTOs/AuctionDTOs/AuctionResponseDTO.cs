using System;
namespace MineralKingdomApi.DTOs.AuctionDTOs
{
    public class AuctionResponseDTO
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? MineralName { get; set; }

        public string? AuctionStatusName { get; set; }

        public int? BidCount { get; set; }

        public int? MineralId { get; set; }
    }

}

