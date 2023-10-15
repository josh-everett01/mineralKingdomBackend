using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.DTOs;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public class AuctionStatusRepository : IAuctionStatusRepository
    {
        private readonly MineralKingdomContext _context;

        public AuctionStatusRepository(MineralKingdomContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<AuctionStatus>> GetAllAuctionStatusesAsync()
        {
            return await _context.AuctionStatuses
                .ToListAsync();
        }

        public async Task<AuctionStatus> GetAuctionStatusByIdAsync(int id)
        {
            return await _context.AuctionStatuses
                                 .FirstOrDefaultAsync(a => a.Id == id)
                                 ?? throw new KeyNotFoundException($"AuctionStatus with ID {id} not found.");
        }


        public async Task CreateAuctionStatusAsync(AuctionStatus auctionStatus)
        {
            if (auctionStatus == null) throw new ArgumentNullException(nameof(auctionStatus));

            await _context.AuctionStatuses.AddAsync(auctionStatus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuctionStatusAsync(AuctionStatus auctionStatus)
        {
            if (auctionStatus == null) throw new ArgumentNullException(nameof(auctionStatus));

            _context.AuctionStatuses.Update(auctionStatus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuctionStatusAsync(int id)
        {
            var auctionStatus = await _context.AuctionStatuses.FindAsync(id);
            if (auctionStatus == null) throw new KeyNotFoundException($"AuctionStatus with ID {id} not found.");

            _context.AuctionStatuses.Remove(auctionStatus);
            await _context.SaveChangesAsync();
        }

        //public async Task<AuctionStatus?> GetAuctionStatusWithAuctionsAsync(int id)
        //{
        //    var auctionStatuses = await _context.AuctionStatuses
        //                         .Include(a => a.Auctions)
        //                         .FirstOrDefaultAsync(a => a.Id == id);
        //    return auctionStatuses;
        //}

        //private AuctionStatusDetailsDTO MapToDetailsDTO(AuctionStatus auctionStatus)
        //{
        //    return new AuctionStatusDetailsDTO
        //    {
        //        Id = auctionStatus.Id ?? 0,  // Handling null, adjust as per your use case
        //        Status = auctionStatus.Status,
        //        // CreatedAt = entity.CreatedAt,  // Not available in your model
        //        // UpdatedAt = entity.UpdatedAt,  // Not available in your model
        //        TotalAuctions = auctionStatus.Auctions?.Count ?? 0,
        //        // ActiveAuctions = entity.Auctions.Count(a => a.IsLive),  // Not available in your model
        //        // CompletedAuctions = entity.Auctions.Count(a => a.IsCompleted),  // Not available in your model
        //        // CreatedBy = entity.CreatedBy,  // Not available in your model
        //        // UpdatedBy = entity.UpdatedBy,  // Not available in your model
        //        Description = auctionStatus.DetailedStatus,  // Assuming DetailedStatus is the description
        //        Auctions = auctionStatus.Auctions?.Select(a => new AuctionDetailsDTO
        //        {
        //            Id = a.Id,
        //            Name = a.Title,  // Using Title as Name
        //            StartTime = a.StartTime,
        //            EndTime = a.EndTime,
        //            Description = a.Description,
        //            // IsLive = a.IsLive,  // Not available in your model
        //            // IsCompleted = a.IsCompleted,  // Not available in your model
        //            // CreatedAt = a.CreatedAt,  // Not available in your model
        //            // CreatedBy = a.CreatedBy,  // Not available in your model
        //            // UpdatedAt = a.UpdatedAt,  // Not available in your model
        //            // UpdatedBy = a.UpdatedBy,  // Not available in your model
        //            Minerals = a.Mineral != null ? new List<MineralResponseDTO>
        //    {
        //        new MineralResponseDTO
        //        {
        //            // Map properties from Mineral to MineralResponseDTO
        //            // Example:
        //            Name = a.Mineral.Name,
        //            Description = a.Mineral.Description,
        //            Price = a.Mineral.Price,
        //            Origin = a.Mineral.Origin,
        //            CreatedAt = a.Mineral.CreatedAt,
        //            ImageURL = a.Mineral.ImageURL
        //            // ... other properties ...
        //        }
        //    } : null,
        //            AuctionStatus = new AuctionStatusResponseDTO
        //            {
        //                Id = auctionStatus.Id ?? 0,
        //                Status = auctionStatus.Status,
        //            }
        //        }).ToList(),
        //    };
        //}

        //public async Task<AuctionStatusDetailsDTO?> GetAuctionStatusWithDetailsAsync(int id)
        //{
        //   var auctionStatus = await _context.AuctionStatuses
        //                         .Include(a => a.Auctions)  // Assuming there is a navigation property named Auctions
        //                         .FirstOrDefaultAsync(a => a.Id == id);

        //    return MapToDetailsDTO(auctionStatus);
        //}

    }
}

