using System;
using MineralKingdomApi.DTOs.AuctionDTOs.YourNamespace.Models;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

namespace MineralKingdomApi.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;

        public AuctionService(IAuctionRepository auctionRepository)
        {
            _auctionRepository = auctionRepository;
        }

        public async Task<IEnumerable<Auction>> GetAllAuctionsAsync()
        {
            return await _auctionRepository.GetAllAuctionsAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            return await _auctionRepository.GetAuctionByIdAsync(id);
        }

        public async Task CreateAuctionAsync(Auction auction)
        {
            await _auctionRepository.CreateAuctionAsync(auction);
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            await _auctionRepository.UpdateAuctionAsync(auction);
        }

        public async Task DeleteAuctionAsync(Auction auction)
        {
            await _auctionRepository.DeleteAuctionAsync(auction);
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId)
        {
            return await _auctionRepository.GetAuctionsByStatusAsync(auctionStatusId);
        }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            return await _auctionRepository.GetActiveAuctionsAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsWithBidsAsync()
        {
            return await _auctionRepository.GetAuctionsWithBidsAsync();
        }

        public async Task<IEnumerable<Bid>> GetBidsForAuctionAsync(int auctionId)
        {
            return await _auctionRepository.GetBidsForAuctionAsync(auctionId);
        }

        public async Task<IEnumerable<Auction>> GetAuctionsForMineralAsync(int mineralId)
        {
            return await _auctionRepository.GetAuctionsForMineralAsync(mineralId);
        }

        public async Task<BidResult> GetWinningBidForCompletedAuction(int auctionId)
        {
            return await _auctionRepository.GetWinningBidForCompletedAuction(auctionId);
        }

        public async Task<BidResult> GetCurrentWinningBidForAuction(int auctionId)
        {
            return await _auctionRepository.GetCurrentWinningBidForAuction(auctionId);
        }
    }

}

