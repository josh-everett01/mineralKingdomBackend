using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.BidDTOs
{
    public class UpdateBidDTO
    {
        [Required]
        public int Id { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? BidTime { get; set; }
    }

}

