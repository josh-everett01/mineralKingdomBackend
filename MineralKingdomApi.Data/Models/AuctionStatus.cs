using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{
    public class AuctionStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }

        // Relationship Navigation Property: An AuctionStatus can be associated with multiple Auctions
        public ICollection<Auction>? Auctions { get; set; }
    }
}
