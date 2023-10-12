using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
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
            return await _context.AuctionStatuses.ToListAsync();
        }

        public async Task<AuctionStatus> GetAuctionStatusByIdAsync(int id)
        {
            return await _context.AuctionStatuses.FindAsync(id)
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
    }
}

