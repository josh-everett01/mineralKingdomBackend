using System;
using MineralKingdomApi.DTOs.AuctionDTOs.YourNamespace.Models;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IAuctionRepository
    {
        Task CreateAuctionAsync(Auction auction);
        Task DeleteAuctionAsync(Auction auction);
        Task<IEnumerable<Auction>> GetAllAuctionsAsync();
        Task<Auction?> GetAuctionByIdAsync(int id);
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();
        Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId);
        Task<IEnumerable<Auction>> GetAuctionsWithBidsAsync();
        Task<IEnumerable<Auction>> GetAuctionsForMineralAsync(int mineralId);
        Task<IEnumerable<Bid>> GetBidsForAuctionAsync(int auctionId);
        Task<BidResult> GetCurrentWinningBidForAuction(int auctionId);
        Task<BidResult> GetWinningBidForCompletedAuction(int auctionId);
        Task UpdateAuctionAsync(Auction auction);
    }

}

