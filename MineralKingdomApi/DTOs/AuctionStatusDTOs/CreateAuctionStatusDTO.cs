using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.AuctionStatusDTOs
{
    public class CreateAuctionStatusDTO
    {
        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }
    }

}

