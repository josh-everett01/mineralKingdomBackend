using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.BidDTOs
{
    public class CreateBidDTO
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime BidTime { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AuctionId { get; set; }
    }

}

