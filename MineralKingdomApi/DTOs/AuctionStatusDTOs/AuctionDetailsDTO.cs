using System;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;

namespace MineralKingdomApi.DTOs.AuctionStatusDTOs
{
    public class AuctionDetailsDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        //public BidResponseDTO WinningBid { get; set; }
        //public UserResponseDTO AssociatedUser { get; set; }
        public AuctionStatusResponseDTO? AuctionStatus { get; set; }
        //public IEnumerable<BidResponseDTO> Bids { get; set; }
        public IEnumerable<MineralResponseDTO>? Minerals { get; set; }
        public string? Description { get; set; }
        public bool IsLive { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

}

