using System;
using MineralKingdomApi.DTOs.AuctionDTOs.YourNamespace.Models;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IAuctionRepository
    {
        // Creates a new auction.
        Task CreateAuctionAsync(Auction auction);

        // Deletes the specified auction.
        Task DeleteAuctionAsync(Auction auction);

        // Retrieves all auctions.
        Task<IEnumerable<Auction>> GetAllAuctionsAsync();

        // Retrieves an auction by its ID.
        Task<Auction?> GetAuctionByIdAsync(int id);

        // Retrieves all active auctions.
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();

        // Retrieves auctions by their status ID.
        Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId);

        // Retrieves auctions that have bids.
        Task<IEnumerable<Auction>> GetAuctionsWithBidsAsync();

        // Retrieves auctions for a specific mineral.
        Task<IEnumerable<Auction>> GetAuctionsForMineralAsync(int mineralId);

        // Retrieves bids for a specific auction.
        Task<IEnumerable<Bid>> GetBidsForAuctionAsync(int auctionId);

        // Retrieves the current winning bid for a specific auction.
        Task<BidResult> GetCurrentWinningBidForAuction(int auctionId);

        // Retrieves the winning bid for a completed auction.
        Task<BidResult> GetWinningBidForCompletedAuction(int auctionId);

        // Updates the specified auction.
        Task UpdateAuctionAsync(Auction auction);

        // Retrieve Auctions that are finished, but whose winners
        // haven't been notified yet
        Task<IEnumerable<Auction>> GetFinishedAndUnnotifiedAuctionsAsync();

        Task<IEnumerable<Bid>> GetProxyBidsForAuction(int AuctionId);
    }


}

