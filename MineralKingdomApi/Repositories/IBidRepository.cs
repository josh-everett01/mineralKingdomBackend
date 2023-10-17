using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IBidRepository
    {
        Task<IEnumerable<Bid>> GetAllBidsAsync();
        Task<Bid> GetBidByIdAsync(int id);
        Task CreateBidAsync(Bid bid);
        Task UpdateBidAsync(Bid bid);
        Task DeleteBidAsync(Bid bid);
    }

}

