using System;
using MineralKingdomApi.DTOs;
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
                Status = auctionStatus.Status,
                DetailedStatus = auctionStatus.DetailedStatus

                // Map other properties as needed
            };
        }

        private AuctionStatus MapDTOToEntity(CreateAuctionStatusDTO createDto)
        {
            return new AuctionStatus
            {
                Status = createDto.Status,
                DetailedStatus = createDto.DetailedStatus
                // Map other properties as needed
            };
        }

        private void MapDTOToEntity(UpdateAuctionStatusDTO updateDto, AuctionStatus auctionStatus)
        {
            auctionStatus.Status = updateDto.Status;
            auctionStatus.DetailedStatus = updateDto.DetailedStatus;
            // Map other properties as needed
        }

        //public async Task<AuctionStatusDetailsDTO> GetAuctionStatusDetailsByIdAsync(int id)
        //{
        //    var auctionStatus = await _repository.GetAuctionStatusByIdAsync(id);
        //    return MapToDetailsDTO(auctionStatus);
        //}

        //public async Task<AuctionStatusDetailsDTO?> GetAuctionStatusDetailsAsync(int id)
        //{
        //    return await _repository.GetAuctionStatusWithDetailsAsync(id);
        //}


        //private AuctionStatusDetailsDTO MapToDetailsDTO(AuctionStatus auctionStatus)
        //{
        //    return new AuctionStatusDetailsDTO
        //    {
        //        Id = auctionStatus.Id ?? 0,
        //        Status = auctionStatus.Status,
        //        TotalAuctions = auctionStatus.Auctions?.Count ?? 0,
        //        Description = auctionStatus.DetailedStatus,
        //        Auctions = auctionStatus.Auctions?.Select(a => new AuctionDetailsDTO
        //        {
        //            Id = a.Id,
        //            Name = a.Title,
        //            StartTime = a.StartTime,
        //            EndTime = a.EndTime,
        //            Description = a.Description,
        //            Minerals = a.Mineral != null ? new List<MineralResponseDTO>
        //    {
        //        new MineralResponseDTO
        //        {
        //            Name = a.Mineral.Name,
        //            Description = a.Mineral.Description,
        //            Price = a.Mineral.Price,
        //            Origin = a.Mineral.Origin,
        //            CreatedAt = a.Mineral.CreatedAt,
        //            ImageURL = a.Mineral.ImageURL
        //        }
        //    } : null,
        //            AuctionStatus = new AuctionStatusResponseDTO
        //            {
        //                Id = auctionStatus.Id ?? 0,
        //                Status = auctionStatus.Status,
        //            }
        //        }).ToList(),
        //   };
        //}
    }
}

