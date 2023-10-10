using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.AuctionDTOs
{
    public class CreateAuctionDTO
    {
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal StartingPrice { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public int MineralId { get; set; }

        [Required]
        public int AuctionStatusId { get; set; }
    }

}

