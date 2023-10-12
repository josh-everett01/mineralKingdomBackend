using System;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;

namespace MineralKingdomApi.Services
{
    public interface IAuctionStatusService
    {
        Task<IEnumerable<AuctionStatusResponseDTO>> GetAllAuctionStatusesAsync();
        Task<AuctionStatusResponseDTO> GetAuctionStatusByIdAsync(int id);
        Task<AuctionStatusResponseDTO> CreateAuctionStatusAsync(CreateAuctionStatusDTO createDto);
        Task<AuctionStatusResponseDTO> UpdateAuctionStatusAsync(int id, UpdateAuctionStatusDTO updateDto);
        Task DeleteAuctionStatusAsync(int id);
    }

}

