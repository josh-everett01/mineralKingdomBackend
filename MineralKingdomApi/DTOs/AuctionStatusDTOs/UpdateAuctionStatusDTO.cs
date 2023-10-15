using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.AuctionStatusDTOs
{
    public class UpdateAuctionStatusDTO
    {
        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Required]
        [MaxLength(500)]
        public string? DetailedStatus { get; set; }
    }

}

