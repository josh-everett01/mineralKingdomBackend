using System;
using MineralKingdomApi.DTOs.AuctionDTOs;
using MineralKingdomApi.DTOs.AuctionDTOs.YourNamespace.Models;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Services
{
    public interface IAuctionService
    {
        Task<IEnumerable<AuctionResponseDTO>> GetAllAuctionsAsync();
        Task<Auction?> GetAuctionByIdAsync(int id);
        Task CreateAuctionAsync(Auction auction);
        Task UpdateAuctionAsync(Auction auction);
        Task DeleteAuctionAsync(Auction auction);
        Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId);
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();
        Task<IEnumerable<Auction>> GetAuctionsWithBidsAsync();
        Task<IEnumerable<Bid>> GetBidsForAuctionAsync(int auctionId);
        Task<IEnumerable<Auction>> GetAuctionsForMineralAsync(int mineralId);
        Task<BidResult> GetWinningBidForCompletedAuction(int auctionId);
        Task<BidResult> GetCurrentWinningBidForAuction(int auctionId);
    }

}

