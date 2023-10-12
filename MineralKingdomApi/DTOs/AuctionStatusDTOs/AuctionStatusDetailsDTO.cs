using System;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;

namespace MineralKingdomApi.DTOs.AuctionStatusDTOs
{
    public class AuctionStatusDetailsDTO : AuctionStatusResponseDTO
    {
        // Additional properties for detailed view
        public new IEnumerable<AuctionDetailsDTO>? Auctions { get; set; }

        // Potential additional properties:
        public DateTime CreatedAt { get; set; } // When the status was created
        public DateTime UpdatedAt { get; set; } // When the status was last updated
        public int TotalAuctions { get; set; } // Total number of auctions with this status
        public int ActiveAuctions { get; set; } // Number of ongoing auctions with this status
        public int CompletedAuctions { get; set; } // Number of completed auctions with this status
        public string? CreatedBy { get; set; } // User who created the status
        public string? UpdatedBy { get; set; } // User who last updated the status
        public string? Description { get; set; } // A description or notes about this status
    }

}

