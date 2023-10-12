using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.DTOs.AuctionDTOs
{
    namespace YourNamespace.Models
    {
        public class BidResult
        {
            public Bid? WinningBid { get; set; }
            public string? Message { get; set; }
            public bool IsSuccess { get; set; }
        }
    }

}

