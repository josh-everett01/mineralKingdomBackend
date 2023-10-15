using System;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IAuctionStatusRepository
    {
        // Retrieve all auction statuses.
        Task<IEnumerable<AuctionStatus>> GetAllAuctionStatusesAsync();

        // Retrieve a specific auction status by ID.
        Task<AuctionStatus> GetAuctionStatusByIdAsync(int id);

        // Add a new auction status.
        Task CreateAuctionStatusAsync(AuctionStatus auctionStatus);

        // Update an existing auction status.
        Task UpdateAuctionStatusAsync(AuctionStatus auctionStatus);

        // Delete an auction status by ID.
        Task DeleteAuctionStatusAsync(int id);

        //Task<AuctionStatus?> GetAuctionStatusWithAuctionsAsync(int id);
        //Task<AuctionStatusDetailsDTO?> GetAuctionStatusWithDetailsAsync(int id);
    }

}

