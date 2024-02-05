using System;
using MineralKingdomApi.DTOs.BidDTOs;

namespace MineralKingdomApi.Services
{
    public interface IBidService
    {
        Task<IEnumerable<BidDTO>> GetAllBidsAsync();
        Task<BidDTO> GetBidByIdAsync(int id);
        Task CreateBidAsync(CreateBidDTO bid);
        Task UpdateBidAsync(UpdateBidDTO bid);
        Task DeleteBidAsync(int id);
        decimal CalculateBidIncrement(decimal currentBid);
        Task ProcessProxyBids();
    }

}

