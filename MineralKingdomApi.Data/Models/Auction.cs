using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MineralKingdomApi.Models
{
    public class Auction
    {
        [Key]
        public int Id { get; set; }

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

        // Relationship Navigation Property: An Auction has one Mineral
        public int? MineralId { get; set; }
        public Mineral? Mineral { get; set; }

        // Relationship Navigation Property: An Auction has one AuctionStatus
        public int? AuctionStatusId { get; set; }

        [JsonIgnore]
        public AuctionStatus? AuctionStatus { get; set; }

        // Relationship Navigation Property: An Auction can have multiple Bids
        public ICollection<Bid>? Bids { get; set; }

        public bool IsWinnerNotified { get; set; }

        [Column(TypeName = "xid")] // Specify the correct PostgreSQL data type
        public uint Xmin { get; set; }

        // Add a RowVersion property
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
