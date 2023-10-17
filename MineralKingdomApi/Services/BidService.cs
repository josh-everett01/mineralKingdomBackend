using System.Collections.Generic;
using System.Threading.Tasks;
using MineralKingdomApi.DTOs.BidDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;

public class BidService : IBidService
{
    private readonly IBidRepository _bidRepository;

    public BidService(IBidRepository bidRepository)
    {
        _bidRepository = bidRepository;
    }

    public async Task<IEnumerable<BidDTO>> GetAllBidsAsync()
    {
        var bids = await _bidRepository.GetAllBidsAsync();
        // Assuming a mapping from Bid to BidDTO, for simplicity using direct mapping here
        return bids.Select(b => new BidDTO
        {
            Id = b.Id,
            Amount = b.Amount,
            BidTime = b.BidTime,
            UserId = b.UserId,
            AuctionId = b.AuctionId
        });
    }

    public async Task<BidDTO> GetBidByIdAsync(int id)
    {
        var bid = await _bidRepository.GetBidByIdAsync(id);
        if (bid == null) return null;

        return new BidDTO
        {
            Id = bid.Id,
            Amount = bid.Amount,
            BidTime = bid.BidTime,
            UserId = bid.UserId,
            AuctionId = bid.AuctionId
        };
    }

    public async Task CreateBidAsync(CreateBidDTO bidDto)
    {
        var bid = new Bid
        {
            Amount = bidDto.Amount,
            BidTime = bidDto.BidTime,
            UserId = bidDto.UserId,
            AuctionId = bidDto.AuctionId
        };
        await _bidRepository.CreateBidAsync(bid);
    }

    public async Task UpdateBidAsync(UpdateBidDTO bidDto)
    {
        var existingBid = await _bidRepository.GetBidByIdAsync(bidDto.Id);
        if (existingBid == null) throw new Exception("Bid not found");

        existingBid.Amount = bidDto.Amount ?? existingBid.Amount;
        existingBid.BidTime = bidDto.BidTime ?? existingBid.BidTime;

        await _bidRepository.UpdateBidAsync(existingBid);
    }

    public async Task DeleteBidAsync(int id)
    {
        var bid = await _bidRepository.GetBidByIdAsync(id);
        if (bid == null) throw new Exception("Bid not found");

        await _bidRepository.DeleteBidAsync(bid);
    }
}
