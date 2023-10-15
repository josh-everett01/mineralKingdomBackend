using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MineralKingdomApi.Models
{
    public class AuctionStatus
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(500)]
        public string? DetailedStatus { get; set; }

        [JsonIgnore]
        public List<Auction>? Auctions { get; set; }
    }
}