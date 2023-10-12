using System;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

namespace MineralKingdomApi.Services
{
    public class AuctionStatusService : IAuctionStatusService
    {
        private readonly IAuctionStatusRepository _repository;

        public AuctionStatusService(IAuctionStatusRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IEnumerable<AuctionStatusResponseDTO>> GetAllAuctionStatusesAsync()
        {
            var auctionStatuses = await _repository.GetAllAuctionStatusesAsync();
            return auctionStatuses.Select(MapToDTO);
        }

        public async Task<AuctionStatusResponseDTO> GetAuctionStatusByIdAsync(int id)
        {
            var auctionStatus = await _repository.GetAuctionStatusByIdAsync(id);
            return MapToDTO(auctionStatus);
        }

        public async Task<AuctionStatusResponseDTO> CreateAuctionStatusAsync(CreateAuctionStatusDTO createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));

            var auctionStatus = MapDTOToEntity(createDto);
            await _repository.CreateAuctionStatusAsync(auctionStatus);
            return MapToDTO(auctionStatus);
        }

        public async Task<AuctionStatusResponseDTO> UpdateAuctionStatusAsync(int id, UpdateAuctionStatusDTO updateDto)
        {
            if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));

            var auctionStatus = await _repository.GetAuctionStatusByIdAsync(id);
            MapDTOToEntity(updateDto, auctionStatus);
            await _repository.UpdateAuctionStatusAsync(auctionStatus);
            return MapToDTO(auctionStatus);
        }

        public async Task DeleteAuctionStatusAsync(int id)
        {
            await _repository.DeleteAuctionStatusAsync(id);
        }

        // Mapping Methods
        private AuctionStatusResponseDTO MapToDTO(AuctionStatus auctionStatus)
        {
            return new AuctionStatusResponseDTO
            {
                Id = auctionStatus.Id ?? 0,  // Handling null, adjust as per your use case
                Status = auctionStatus.Status
                // Map other properties as needed
            };
        }

        private AuctionStatus MapDTOToEntity(CreateAuctionStatusDTO createDto)
        {
            return new AuctionStatus
            {
                Status = createDto.Status
                // Map other properties as needed
            };
        }

        private void MapDTOToEntity(UpdateAuctionStatusDTO updateDto, AuctionStatus auctionStatus)
        {
            auctionStatus.Status = updateDto.Status;
            // Map other properties as needed
        }
    }

}

