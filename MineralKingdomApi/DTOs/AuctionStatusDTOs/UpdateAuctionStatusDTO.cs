using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.AuctionStatusDTOs
{
    public class UpdateAuctionStatusDTO
    {
        [Required]
        [MaxLength(50)]
        public int Id { get; set; }
        public string? Status { get; set; }
    }

}

