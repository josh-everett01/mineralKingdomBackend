using System;
using MineralKingdomApi.DTOs.AuctionDTOs;
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

        public async Task<IEnumerable<AuctionResponseDTO>> GetAllAuctionsAsync()
        {
            var auctions = await _auctionRepository.GetAllAuctionsAsync();
            return auctions.Select(a => MapToDTO(a));
        }

        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            return await _auctionRepository.GetAuctionByIdAsync(id);
        }


        public async Task CreateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            await _auctionRepository.CreateAuctionAsync(auction);
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }

            await _auctionRepository.UpdateAuctionAsync(auction);
        }

        public async Task DeleteAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            await _auctionRepository.DeleteAuctionAsync(auction);
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId)
        {
            var auctions = await _auctionRepository.GetAuctionsByStatusAsync(auctionStatusId);
            return auctions ?? Enumerable.Empty<Auction>();
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
            var bids = await _auctionRepository.GetBidsForAuctionAsync(auctionId);
    return bids ?? Enumerable.Empty<Bid>();
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

        private AuctionResponseDTO MapToDTO(Auction auction)
        {
            return new AuctionResponseDTO
            {
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                StartingPrice = auction.StartingPrice,
                StartTime = auction.StartTime,
                EndTime = auction.EndTime,
                MineralName = auction.Mineral?.Name,  // Assuming Mineral has a Name property
                AuctionStatusName = auction.AuctionStatus?.Status,  // Assuming AuctionStatus has a Name property
                BidCount = auction.Bids?.Count ?? 0  // Assuming Auction has a Bids collection
            };
        }

        //private Auction MapToModel(CreateAuctionDTO auctionDto)
        //{
        //    return new Auction
        //    {
        //        Title = auctionDto.Title,
        //        Description = auctionDto.Description,
        //        StartingPrice = auctionDto.StartingPrice,
        //        StartTime = auctionDto.StartTime,
        //        EndTime = auctionDto.EndTime,
        //        MineralId = auctionDto.MineralId,
        //        AuctionStatusId = auctionDto.AuctionStatusId
        //    };
        //}
    }
}

