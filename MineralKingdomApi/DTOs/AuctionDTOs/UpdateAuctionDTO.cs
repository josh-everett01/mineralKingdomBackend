using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.AuctionDTOs
{
    public class UpdateAuctionDTO
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal? StartingPrice { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? MineralId { get; set; }

        public int? AuctionStatusId { get; set; }
    }

}

