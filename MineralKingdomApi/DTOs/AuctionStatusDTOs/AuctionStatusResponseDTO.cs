using System;
using MineralKingdomApi.DTOs.AuctionDTOs;

namespace MineralKingdomApi.DTOs.AuctionStatusDTOs
{
    public class AuctionStatusResponseDTO
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public IEnumerable<AuctionResponseDTO> Auctions { get; set; } = new List<AuctionResponseDTO>();
    }


}

