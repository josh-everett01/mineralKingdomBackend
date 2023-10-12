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

        public string? MineralName { get; set; } // Assuming Mineral has a Name property

        public string? AuctionStatusName { get; set; } // Assuming AuctionStatus has a Name property

        public int? BidCount { get; set; } // Number of bids on the auction
    }

}

